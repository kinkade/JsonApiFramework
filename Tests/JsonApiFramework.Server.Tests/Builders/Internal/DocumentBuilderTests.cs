﻿// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using JsonApiFramework.Http;
using JsonApiFramework.Internal;
using JsonApiFramework.Internal.Dom;
using JsonApiFramework.Json;
using JsonApiFramework.JsonApi;
using JsonApiFramework.Server.Hypermedia;
using JsonApiFramework.Server.Hypermedia.Internal;
using JsonApiFramework.Server.Internal;
using JsonApiFramework.ServiceModel;
using JsonApiFramework.TestAsserts.JsonApi;
using JsonApiFramework.TestData.ApiResources;
using JsonApiFramework.TestData.ClrResources;
using JsonApiFramework.XUnit;

using Xunit;
using Xunit.Abstractions;

namespace MyNamespace
{
    using JsonApiFramework;
    using JsonApiFramework.JsonApi;
    using JsonApiFramework.Server;
    using JsonApiFramework.TestData.ApiResources;
    using JsonApiFramework.TestData.ClrResources;

    public static class MyClass
    {
        public static void MyMethod()
        {
            var resourceDocument = new DocumentContext(default(IDocumentContextOptions))
                .NewDocument(String.Empty)
                    .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                    .SetMeta(ApiSampleData.DocumentMeta)
                    .Links()
                        .AddUpLink()
                        .AddSelfLink()
                    .LinksEnd()
                    .Resource(SampleArticles.Article)
                        .SetMeta(ApiSampleData.ResourceMeta)
                        .Relationships()
                            .AddRelationship(ApiSampleData.ArticleToAuthorRel, Keywords.Self, Keywords.Related)
                            .AddRelationship(ApiSampleData.ArticleToCommentsRel, Keywords.Self, Keywords.Related)
                            .Relationship(ApiSampleData.ArticleToCommentsRel)
                                .Links()
                                    .AddSelfLink()
                                    .AddRelatedLink()
                                .LinksEnd()
                            .RelationshipEnd()
                        .RelationshipsEnd()
                        .Links()
                            .AddSelfLink()
                            .AddCanonicalLink()
                            .AddSelfLink(x => x.Title.Any())
                            .AddCanonicalLink(x => x.Title.Any())
                        .LinksEnd()
                    .ResourceEnd()
                    .Included()
                        .ToOne(SampleArticles.Article, ApiSampleData.ArticleToAuthorRel, SamplePersons.Person)
                            .SetMeta(ApiSampleData.ResourceMeta)
                            .Relationships()
                                .AddRelationship(ApiSampleData.PersonToCommentsRel, Keywords.Self, Keywords.Related)
                            .RelationshipsEnd()
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                        .ToOneEnd()

                        .ToMany(SampleArticles.Article, ApiSampleData.ArticleToCommentsRel, new[] { SampleComments.Comment1, SampleComments.Comment2 })
                            .SetMeta(ApiSampleData.ResourceMeta)
                            .Relationships()
                                .AddRelationship(ApiSampleData.CommentToAuthorRel, Keywords.Self, Keywords.Related)
                            .RelationshipsEnd()
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                        .ToManyEnd()
                    .IncludedEnd()
                .WriteDocument();

            var errorsDocument = new DocumentContext(default(IDocumentContextOptions))
                .NewDocument(String.Empty)
                    .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                    .SetMeta(ApiSampleData.DocumentMeta)
                    .Links()
                        .AddLink(Keywords.Self)
                    .LinksEnd()
                    .Errors()
                        .AddError(ApiSampleData.Error1)
                        .AddError(ApiSampleData.Error2)
                    .ErrorsEnd()
                .WriteDocument();
        }
    }
}

namespace JsonApiFramework.Server.Tests.Internal
{
    public class DocumentBuilderTests : XUnitTest
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public DocumentBuilderTests(ITestOutputHelper output)
            : base(output)
        { }
        #endregion

        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Test Methods
        [Theory]
        [MemberData("DocumentBuilderWriteDocumentTestData")]
        public void TestDocumentBuilderWriteDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteEmptyDocumentTestData")]
        public void TestDocumentBuilderWriteEmptyDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteNullDocumentTestData")]
        public void TestDocumentBuilderWriteNullDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteResourceCollectionDocumentTestData")]
        public void TestDocumentBuilderWriteResourceCollectionDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteResourceDocumentTestData")]
        public void TestDocumentBuilderWriteResourceDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteResourceIdentifierCollectionDocumentTestData")]
        public void TestDocumentBuilderWriteResourceIdentifierCollectionDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteResourceIdentifierDocumentTestData")]
        public void TestDocumentBuilderWriteResourceIdentifierDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteWithFrameworkHypermediaTestData")]
        public void TestDocumentBuilderWriteWithFrameworkHypermedia(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteWithPredicateBasedFrameworkHypermediaTestData")]
        public void TestDocumentBuilderWriteWithPredicateBasedFrameworkHypermedia(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }

        [Theory]
        [MemberData("DocumentBuilderWriteErrorsDocumentTestData")]
        public void TestDocumentBuilderWriteErrorsDocument(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        { this.TestDocumentBuilderWrite(name, expectedDocument, actualDocumentWriter); }
        #endregion

        // PRIVATE METHODS //////////////////////////////////////////////////
        #region Methods
        private void OutputEmptyLine()
        {
            this.Output.WriteLine(String.Empty);
        }

        private void OutputJson(string header, IJsonObject jsonObject)
        {
            var json = jsonObject.ToJson();

            this.Output.WriteLine(header);
            this.Output.WriteLine(String.Empty);
            this.Output.WriteLine(json);
        }

        private void TestDocumentBuilderWrite(string name, Document expectedDocument, IDocumentWriter actualDocumentWriter)
        {
            // Arrange

            // Act
            var hasDomDocument = (IGetDomDocument)actualDocumentWriter;

            this.Output.WriteLine("Test Name: {0}", name);
            this.OutputEmptyLine();
            this.Output.WriteLine("Before WriteDocument Method DOM Tree");
            this.OutputEmptyLine();
            this.Output.WriteLine(hasDomDocument.DomDocument.ToTreeString());
            this.OutputEmptyLine();

            var actualDocument = actualDocumentWriter.WriteDocument();

            this.Output.WriteLine("After WriteDocument Method DOM Tree");
            this.OutputEmptyLine();
            this.Output.WriteLine(hasDomDocument.DomDocument.ToTreeString());
            this.OutputEmptyLine();

            this.OutputJson("Expected Document JSON", expectedDocument);
            this.OutputEmptyLine();

            this.OutputJson("Actual Document JSON", actualDocument);

            // Assert
            DocumentAssert.Equal(expectedDocument, actualDocument);
        }
        #endregion

        // PUBLIC FIELDS ////////////////////////////////////////////////////
        #region Test Data
        public static readonly UrlBuilderConfiguration UrlBuilderConfigurationWithRootPathSegments = new UrlBuilderConfiguration
            {
                Scheme = "http",
                Host = "api.example.com",
                RootPathSegments = new[]
                    {
                        "api",
                        "v2"
                    }
            };

        public static readonly UrlBuilderConfiguration UrlBuilderConfigurationWithoutRootPathSegments = new UrlBuilderConfiguration
            {
                Scheme = "http",
                Host = "api.example.com"
            };

        public static readonly IHypermediaAssemblerRegistry HypermediaAssemblerRegistry = default(IHypermediaAssemblerRegistry);

        private static class DocumentBuilderFactory
        {
            public static DocumentBuilder Create(IServiceModel serviceModel, IHypermediaAssemblerRegistry hypermediaAssemblerRegistry, IUrlBuilderConfiguration urlBuilderConfiguration, string currentRequestUrl)
            {
                var documentWriter = new DocumentWriter(serviceModel);
                var hypermediaContext = new HypermediaContext(serviceModel, urlBuilderConfiguration);
                var documentBuilderContext = new DocumentBuilderContext(currentRequestUrl);
                var documentBuilder = new DocumentBuilder(documentWriter, hypermediaAssemblerRegistry, hypermediaContext, documentBuilderContext);
                return documentBuilder;
            }
        }

        #region DocumentBuilderWriteDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithNothing",
                        Document.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com")
                    },
                new object[]
                    {
                        "WithMeta",
                        new Document
                            {
                                Meta = ApiSampleData.DocumentMeta
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com")
                            .SetMeta(ApiSampleData.DocumentMeta)
                    },
                new object[]
                    {
                        "WithMetaAndLinks",
                        new Document
                            {
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com")
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddSelfLink(ApiSampleData.ArticleLink)
                            .LinksEnd()
                    },
                new object[]
                    {
                        "WithJsonApiAndMetaAndLinks",
                        new Document
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com")
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddSelfLink(ApiSampleData.ArticleLink)
                            .LinksEnd()
                    },
            };
        #endregion


        #region DocumentBuilderWriteEmptyDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteEmptyDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithNothing",
                        EmptyDocument.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/articles")
                            .ResourceCollection(Enumerable.Empty<Article>())
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithJsonApiAndMetaAndLinks",
                        new EmptyDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/articles")
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleCollectionLink)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Article>())
                            .ResourceCollectionEnd()
                    },
            };
        #endregion

        #region DocumentBuilderWriteNullDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteNullDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithNothing",
                        NullDocument.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com")
                            .Resource(default(Article))
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithJsonApiAndMetaAndLinks",
                        new NullDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com")
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                            .LinksEnd()
                            .Resource(default(Article))
                            .ResourceEnd()
                    },
            };
        #endregion


        #region DocumentBuilderWriteResourceCollectionDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteResourceCollectionDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithEmptyResources",
                        ResourceCollectionDocument.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .ResourceCollection(Enumerable.Empty<Article>())
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithEmptyResourcesAndJsonApiAndMetaAndLinks",
                        new ResourceCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Data = Enumerable.Empty<Resource>()
                                                 .ToList()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleCollectionLink)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Article>())
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourcesAndUserBuiltHypermedia",
                        new ResourceCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Data = new List<Resource>
                                    {
                                        ApiSampleData.ArticleResource1,
                                        ApiSampleData.ArticleResource2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleCollectionLink)
                            .LinksEnd()
                            .ResourceCollection(new List<Article> { SampleArticles.Article1, SampleArticles.Article2 })
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, ApiSampleData.ArticleToAuthorRelationship1, ApiSampleData.ArticleToAuthorRelationship2)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, ApiSampleData.ArticleToCommentsRelationship1, ApiSampleData.ArticleToCommentsRelationship2)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self, ApiSampleData.ArticleLink1, ApiSampleData.ArticleLink2)
                                    .AddLink(Keywords.Canonical, ApiSampleData.ArticleLink1, ApiSampleData.ArticleLink2)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourcesAndFrameworkBuiltHypermedia",
                        new ResourceCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Data = new List<Resource>
                                    {
                                        ApiSampleData.ArticleResource1,
                                        ApiSampleData.ArticleResource2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SampleArticles.Article1, SampleArticles.Article2)
                                .SetMeta(ApiSampleData.ResourceMeta, ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                    .AddLink(Keywords.Canonical)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourcesAndIncludedResourcesAndUserBuiltHypermedia",
                        new ResourceCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Data = new List<Resource>
                                    {
                                        ApiSampleData.ArticleResourceWithResourceLinkage1,
                                        ApiSampleData.ArticleResourceWithResourceLinkage2
                                    },
                                Included = new List<Resource>
                                    {
                                        ApiSampleData.PersonResource1,
                                        ApiSampleData.PersonResource2,
                                        ApiSampleData.CommentResource1,
                                        ApiSampleData.CommentResource2,
                                        ApiSampleData.CommentResource3,
                                        ApiSampleData.CommentResource4
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SampleArticles.Article1, SampleArticles.Article2)
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, ApiSampleData.ArticleToAuthorToOneRelationship1, ApiSampleData.ArticleToAuthorToOneRelationship2)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, ApiSampleData.ArticleToCommentsToManyRelationship1, ApiSampleData.ArticleToCommentsToManyRelationship2)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self, ApiSampleData.ArticleLink1, ApiSampleData.ArticleLink2)
                                    .AddLink(Keywords.Canonical, ApiSampleData.ArticleLink1, ApiSampleData.ArticleLink2)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                            .Included()
                                .ToOne(ToOneResourceLinkage.Create(SampleArticles.Article1, ApiSampleData.ArticleToAuthorRel, SamplePersons.Person1),
                                        ToOneResourceLinkage.Create(SampleArticles.Article2, ApiSampleData.ArticleToAuthorRel, SamplePersons.Person2))
                                    .SetMeta(ApiSampleData.ResourceMeta)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.PersonToCommentsRel, ApiSampleData.PersonToCommentsRelationship1, ApiSampleData.PersonToCommentsRelationship2)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self, ApiSampleData.PersonLink1, ApiSampleData.PersonLink2)
                                    .LinksEnd()
                                .ToOneEnd()
                                .ToMany(ToManyResourceLinkage.Create(SampleArticles.Article1, ApiSampleData.ArticleToCommentsRel, SampleComments.Comment1, SampleComments.Comment2),
                                        ToManyResourceLinkage.Create(SampleArticles.Article2, ApiSampleData.ArticleToCommentsRel, SampleComments.Comment3, SampleComments.Comment4))
                                    .SetMeta(ApiSampleData.ResourceMeta1, ApiSampleData.ResourceMeta2, ApiSampleData.ResourceMeta3, ApiSampleData.ResourceMeta4)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.CommentToAuthorRel, ApiSampleData.CommentToAuthorRelationship1, ApiSampleData.CommentToAuthorRelationship2, ApiSampleData.CommentToAuthorRelationship3, ApiSampleData.CommentToAuthorRelationship4)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self, ApiSampleData.CommentLink1, ApiSampleData.CommentLink2, ApiSampleData.CommentLink3, ApiSampleData.CommentLink4)
                                    .LinksEnd()
                                .ToManyEnd()
                            .IncludedEnd()
                    },
                new object[]
                    {
                        "WithResourcesAndIncludedResourcesAndFrameworkBuiltHypermedia",
                        new ResourceCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Data = new List<Resource>
                                    {
                                        ApiSampleData.ArticleResourceWithResourceLinkage1,
                                        ApiSampleData.ArticleResourceWithResourceLinkage2
                                    },
                                Included = new List<Resource>
                                    {
                                        ApiSampleData.PersonResource1,
                                        ApiSampleData.PersonResource2,
                                        ApiSampleData.CommentResource1,
                                        ApiSampleData.CommentResource2,
                                        ApiSampleData.CommentResource3,
                                        ApiSampleData.CommentResource4
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SampleArticles.Article1, SampleArticles.Article2)
                                .SetMeta(ApiSampleData.ResourceMeta, ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                    .AddLink(Keywords.Canonical)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                            .Included()
                                .ToOne(ToOneResourceLinkage.Create(SampleArticles.Article1, ApiSampleData.ArticleToAuthorRel, SamplePersons.Person1),
                                       ToOneResourceLinkage.Create(SampleArticles.Article2, ApiSampleData.ArticleToAuthorRel, SamplePersons.Person2))
                                    .SetMeta(ApiSampleData.ResourceMeta, ApiSampleData.ResourceMeta)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.PersonToCommentsRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()
                                .ToMany(ToManyResourceLinkage.Create(SampleArticles.Article1, ApiSampleData.ArticleToCommentsRel, SampleComments.Comment1, SampleComments.Comment2),
                                        ToManyResourceLinkage.Create(SampleArticles.Article2, ApiSampleData.ArticleToCommentsRel, SampleComments.Comment3, SampleComments.Comment4))
                                    .SetMeta(ApiSampleData.ResourceMeta1, ApiSampleData.ResourceMeta2, ApiSampleData.ResourceMeta3, ApiSampleData.ResourceMeta4)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.CommentToAuthorRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToManyEnd()
                            .IncludedEnd()
                    },
                new object[]
                    {
                        "WithEmptyResourcesAndUserBuiltHypermedia",
                        new ResourceCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Data = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleCollectionLink)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Article>())
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, ApiSampleData.ArticleToAuthorRelationship1, ApiSampleData.ArticleToAuthorRelationship2)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, ApiSampleData.ArticleToCommentsRelationship1, ApiSampleData.ArticleToCommentsRelationship2)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self, ApiSampleData.ArticleLink1, ApiSampleData.ArticleLink2)
                                    .AddLink(Keywords.Canonical, ApiSampleData.ArticleLink1, ApiSampleData.ArticleLink2)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithEmptyResourcesAndFrameworkBuiltHypermedia",
                        new ResourceCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Data = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Article>())
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                    .AddLink(Keywords.Canonical)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
            };
        #endregion

        #region DocumentBuilderWriteResourceDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteResourceDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithNullResource",
                        ResourceDocument.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .Resource(default(Article))
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithNullResourceAndJsonApiAndMetaAndLinks",
                        new ResourceDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                            .LinksEnd()
                            .Resource(default(Article))
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithResourceAndUserBuiltHypermedia",
                        new ResourceDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                                Data = ApiSampleData.ArticleResource
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                            .LinksEnd()
                            .Resource(SampleArticles.Article)
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, ApiSampleData.ArticleToAuthorRelationship)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, ApiSampleData.ArticleToCommentsRelationship)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                                    .AddLink(Keywords.Canonical, ApiSampleData.ArticleLink)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithResourceAndFrameworkBuiltHypermedia",
                        new ResourceDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                                Data = ApiSampleData.ArticleResource
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleArticles.Article)
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                    .AddLink(Keywords.Canonical)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithResourceAndIncludedResourcesAndUserBuiltHypermedia",
                        new ResourceDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                                Data = ApiSampleData.ArticleResourceWithResourceLinkage,
                                Included = new List<Resource>
                                    {
                                        ApiSampleData.PersonResource,
                                        ApiSampleData.CommentResource1,
                                        ApiSampleData.CommentResource2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                            .LinksEnd()
                            .Resource(SampleArticles.Article)
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, ApiSampleData.ArticleToAuthorToOneRelationship)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, ApiSampleData.ArticleToCommentsToManyRelationship)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                                    .AddLink(Keywords.Canonical, ApiSampleData.ArticleLink)
                                .LinksEnd()
                            .ResourceEnd()
                            .Included()
                                .ToOne(SampleArticles.Article, ApiSampleData.ArticleToAuthorRel, SamplePersons.Person)
                                    .SetMeta(ApiSampleData.ResourceMeta)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.PersonToCommentsRel, ApiSampleData.PersonToCommentsRelationship)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self, ApiSampleData.PersonLink)
                                    .LinksEnd()
                                .ToOneEnd()
                                .ToMany(SampleArticles.Article, ApiSampleData.ArticleToCommentsRel, new[]{ SampleComments.Comment1, SampleComments.Comment2 })
                                    .SetMeta(ApiSampleData.ResourceMeta1, ApiSampleData.ResourceMeta2)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.CommentToAuthorRel, ApiSampleData.CommentToAuthorRelationship1, ApiSampleData.CommentToAuthorRelationship2)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self, ApiSampleData.CommentLink1, ApiSampleData.CommentLink2)
                                    .LinksEnd()
                                .ToManyEnd()
                            .IncludedEnd()
                    },
                new object[]
                    {
                        "WithResourceAndIncludedResourcesAndFrameworkBuiltHypermedia",
                        new ResourceDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                                Data = ApiSampleData.ArticleResourceWithResourceLinkage,
                                Included = new List<Resource>
                                    {
                                        ApiSampleData.PersonResource,
                                        ApiSampleData.CommentResource1,
                                        ApiSampleData.CommentResource2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleArticles.Article)
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                    .AddLink(Keywords.Canonical)
                                .LinksEnd()
                            .ResourceEnd()
                            .Included()
                                .ToOne(SampleArticles.Article, ApiSampleData.ArticleToAuthorRel, SamplePersons.Person)
                                    .SetMeta(ApiSampleData.ResourceMeta)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.PersonToCommentsRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()
                                .ToMany(SampleArticles.Article, ApiSampleData.ArticleToCommentsRel, new[]{ SampleComments.Comment1, SampleComments.Comment2 })
                                    .SetMeta(ApiSampleData.ResourceMeta1, ApiSampleData.ResourceMeta2)
                                    .Relationships()
                                        .AddRelationship(ApiSampleData.CommentToAuthorRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToManyEnd()
                            .IncludedEnd()
                    },
                new object[]
                    {
                        "WithNullResourceAndUserBuiltHypermedia",
                        new ResourceDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                            .LinksEnd()
                            .Resource(default(Article))
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, ApiSampleData.ArticleToAuthorRelationship)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, ApiSampleData.ArticleToCommentsRelationship)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self, ApiSampleData.ArticleLink)
                                    .AddLink(Keywords.Canonical, ApiSampleData.ArticleLink)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithNullResourceAndFrameworkBuiltHypermedia",
                        new ResourceDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(default(Article))
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .Relationships()
                                    .AddRelationship(ApiSampleData.ArticleToAuthorRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ApiSampleData.ArticleToCommentsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                    .AddLink(Keywords.Canonical)
                                .LinksEnd()
                            .ResourceEnd()
                    },

                new object[]
                    {
                        "WithNestedComplexTypes",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, UrlBuilder.Create(UrlBuilderConfigurationWithoutRootPathSegments).Path(ClrSampleData.DrawingCollectionPathSegment).Path(SampleDrawings.Drawing.Id).Build()}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.DrawingType,
                                        Id = SampleDrawings.Drawing.Id.ToString(CultureInfo.InvariantCulture),
                                        Attributes = new ApiObject(
                                            ApiProperty.Create("name", SampleDrawings.Drawing.Name),
                                            ApiProperty.Create("lines", SampleDrawings.Drawing.Lines
                                                .Select(x =>
                                                    {
                                                        var point1CustomData = ApiProperty.Create("custom-data",
                                                            x.Point1.CustomData != null
                                                                ? new ApiObject(ApiProperty.Create("collection",
                                                                    x.Point1.CustomData.Collection.EmptyIfNull()
                                                                     .Select(y =>
                                                                         {
                                                                             var apiObject2 = new ApiObject(ApiProperty.Create("name", y.Name), ApiProperty.Create("value", y.Value));
                                                                             return apiObject2;
                                                                         })
                                                                     .ToArray()))
                                                                : null);
                                                        var point1 = ApiProperty.Create("point1", new ApiObject(ApiProperty.Create("x", x.Point1.X), ApiProperty.Create("y", x.Point1.Y), point1CustomData));

                                                        var point2CustomData = ApiProperty.Create("custom-data",
                                                            x.Point2.CustomData != null
                                                                ? new ApiObject(ApiProperty.Create("collection",
                                                                    x.Point2.CustomData.Collection.EmptyIfNull()
                                                                     .Select(y =>
                                                                         {
                                                                             var apiObject2 = new ApiObject(ApiProperty.Create("name", y.Name), ApiProperty.Create("value", y.Value));
                                                                             return apiObject2;
                                                                         })
                                                                     .ToArray()))
                                                                : null);
                                                        var point2 = ApiProperty.Create("point2", new ApiObject(ApiProperty.Create("x", x.Point2.X), ApiProperty.Create("y", x.Point2.Y), point2CustomData));

                                                        var customData = ApiProperty.Create("custom-data",
                                                            x.CustomData != null
                                                                ? new ApiObject(ApiProperty.Create("collection",
                                                                    x.CustomData.Collection.EmptyIfNull()
                                                                     .Select(y =>
                                                                     {
                                                                         var apiObject2 = new ApiObject(ApiProperty.Create("name", y.Name), ApiProperty.Create("value", y.Value));
                                                                         return apiObject2;
                                                                     })
                                                                     .ToArray()))
                                                                : null);
                                                        return new ApiObject(point1, point2, customData);
                                                    })
                                                .ToArray()),
                                            ApiProperty.Create("polygons", SampleDrawings.Drawing.Polygons
                                                .Select(x =>
                                                    {
                                                        var points = ApiProperty.Create("points",
                                                            x.Points.Select(y =>
                                                                {
                                                                    var pointCustomData = ApiProperty.Create("custom-data",
                                                                        y.CustomData != null
                                                                            ? new ApiObject(ApiProperty.Create("collection",
                                                                                y.CustomData.Collection.EmptyIfNull()
                                                                                 .Select(z =>
                                                                                     {
                                                                                         var apiObject3 = new ApiObject(ApiProperty.Create("name", z.Name), ApiProperty.Create("value", z.Value));
                                                                                         return apiObject3;
                                                                                     })
                                                                                 .ToArray()))
                                                                            : null);
                                                                    var apiObject2 = new ApiObject(ApiProperty.Create("x", y.X), ApiProperty.Create("y", y.Y), pointCustomData);
                                                                    return apiObject2;
                                                                })
                                                             .ToArray());
                                                        var customData = ApiProperty.Create("custom-data",
                                                            x.CustomData != null
                                                                ? new ApiObject(ApiProperty.Create("collection",
                                                                    x.CustomData.Collection.EmptyIfNull()
                                                                     .Select(y =>
                                                                     {
                                                                         var apiObject2 = new ApiObject(ApiProperty.Create("name", y.Name), ApiProperty.Create("value", y.Value));
                                                                         return apiObject2;
                                                                     })
                                                                     .ToArray()))
                                                                : null);
                                                        return new ApiObject(points, customData);
                                                    })
                                                .ToArray()),
                                            ApiProperty.Create("custom-data", SampleDrawings.Drawing.CustomData != null
                                                ? new ApiObject(ApiProperty.Create("collection", SampleDrawings.Drawing.CustomData.Collection.EmptyIfNull().Select(x =>
                                                            {
                                                                var apiObject = new ApiObject(ApiProperty.Create("name", x.Name), ApiProperty.Create("value", x.Value));
                                                                return apiObject;
                                                            })
                                                        .ToArray()))
                                                : null)
                                        ),
                                        Links = new Links
                                            {
                                                {Keywords.Self, UrlBuilder.Create(UrlBuilderConfigurationWithoutRootPathSegments).Path(ClrSampleData.DrawingCollectionPathSegment).Path(SampleDrawings.Drawing.Id).Build()}
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithDrawingResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, UrlBuilder.Create(UrlBuilderConfigurationWithoutRootPathSegments).Path(ClrSampleData.DrawingCollectionPathSegment).Path(SampleDrawings.Drawing.Id).Build())
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleDrawings.Drawing)
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },

            };
        #endregion


        #region DocumentBuilderWriteResourceIdentifierCollectionDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteResourceIdentifierCollectionDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithEmptyObject",
                        ResourceIdentifierCollectionDocument.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetResourceIdentifierCollection<Article>()
                    },
                new object[]
                    {
                        "WithNullResourceIdentifiersAndUserBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink)
                            .LinksEnd()
                            .SetResourceIdentifierCollection<Article>()
                    },
                new object[]
                    {
                        "WithNullResourceIdentifiersAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifierCollection<Article>()
                    },
                new object[]
                    {
                        "WithEmptyResourceIdentifiersAndUserBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(Enumerable.Empty<Comment>())
                    },
                new object[]
                    {
                        "WithEmptyResourceIdentifiersAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(Enumerable.Empty<Comment>())
                    },
                new object[]
                    {
                        "WithResourceIdentifiersAndUserBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>
                                    {
                                        ApiSampleData.CommentResourceIdentifier1,
                                        ApiSampleData.CommentResourceIdentifier2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink)
                            .LinksEnd()
                            .ResourceIdentifierCollection<Comment>()
                                .SetId(ApiSampleData.CommentId1, ApiSampleData.CommentId2)
                            .ResourceIdentifierCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourceIdentifiersAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>
                                    {
                                        ApiSampleData.CommentResourceIdentifier1,
                                        ApiSampleData.CommentResourceIdentifier2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceIdentifierCollection<Comment>()
                                .SetId(ApiSampleData.CommentId1, ApiSampleData.CommentId2)
                            .ResourceIdentifierCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourceIdentifiersAndMetaAndUserBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>
                                    {
                                        ApiSampleData.CommentResourceIdentifierWithMeta1,
                                        ApiSampleData.CommentResourceIdentifierWithMeta2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink)
                            .LinksEnd()
                            .ResourceIdentifierCollection<Comment>()
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .SetId(ApiSampleData.CommentId1, ApiSampleData.CommentId2)
                            .ResourceIdentifierCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourceIdentifiersAndMetaAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>
                                    {
                                        ApiSampleData.CommentResourceIdentifierWithMeta1,
                                        ApiSampleData.CommentResourceIdentifierWithMeta2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceIdentifierCollection<Comment>()
                                .SetMeta(ApiSampleData.ResourceMeta, ApiSampleData.ResourceMeta)
                                .SetId(ApiSampleData.CommentId1, ApiSampleData.CommentId2)
                            .ResourceIdentifierCollectionEnd()
                    },
                new object[]
                    {
                        "WithNullResourcesAndUserBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(default(Comment), default(Comment))
                    },
                new object[]
                    {
                        "WithNullResourcesAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(default(Comment), default(Comment))
                    },
                new object[]
                    {
                        "WithEmptyResourcesAndUserBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(new Comment(), new Comment())
                    },
                new object[]
                    {
                        "WithEmptyResourcesAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(new Comment(), new Comment())
                    },
                new object[]
                    {
                        "WithResourcesAndUserBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>
                                    {
                                        ApiSampleData.CommentResourceIdentifier1,
                                        ApiSampleData.CommentResourceIdentifier2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(SampleComments.Comment1, SampleComments.Comment2)
                    },
                new object[]
                    {
                        "WithResourcesAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierCollectionDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToCommentsLink}
                                    },
                                Data = new List<ResourceIdentifier>
                                    {
                                        ApiSampleData.CommentResourceIdentifier1,
                                        ApiSampleData.CommentResourceIdentifier2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToCommentsHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifierCollection(SampleComments.Comment1, SampleComments.Comment2)
                    },
            };
        #endregion

        #region DocumentBuilderWriteResourceIdentifierDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteResourceIdentifierDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithEmptyObject",
                        ResourceIdentifierDocument.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleHRef)
                            .SetResourceIdentifier<Article>()
                    },
                new object[]
                    {
                        "WithNullResourceIdentifierAndUserBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink)
                            .LinksEnd()
                            .SetResourceIdentifier<Person>()
                    },
                new object[]
                    {
                        "WithNullResourceIdentifierAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifier<Person>()
                    },
                new object[]
                    {
                        "WithResourceIdentifierAndUserBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifier
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink)
                            .LinksEnd()
                            .ResourceIdentifier<Person>()
                                .SetId(ApiSampleData.PersonId)
                            .ResourceIdentifierEnd()
                    },
                new object[]
                    {
                        "WithResourceIdentifierAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifier
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceIdentifier<Person>()
                                .SetId(ApiSampleData.PersonId)
                            .ResourceIdentifierEnd()
                    },
                new object[]
                    {
                        "WithResourceIdentifierAndMetaAndUserBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifierWithMeta
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink)
                            .LinksEnd()
                            .ResourceIdentifier<Person>()
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .SetId(ApiSampleData.PersonId)
                            .ResourceIdentifierEnd()
                    },
                new object[]
                    {
                        "WithResourceIdentifierAndMetaAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifierWithMeta
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceIdentifier<Person>()
                                .SetMeta(ApiSampleData.ResourceMeta)
                                .SetId(ApiSampleData.PersonId)
                            .ResourceIdentifierEnd()
                    },
                new object[]
                    {
                        "WithNullResourceAndUserBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink)
                            .LinksEnd()
                            .SetResourceIdentifier(default(Person))
                    },
                new object[]
                    {
                        "WithNullResourceAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifier(default(Person))
                    },
                new object[]
                    {
                        "WithEmptyResourceAndUserBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink)
                            .LinksEnd()
                            .SetResourceIdentifier(new Person())
                    },
                new object[]
                    {
                        "WithEmptyResourceAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifier(new Person())
                    },
                new object[]
                    {
                        "WithResourceAndUserBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifier
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink)
                            .LinksEnd()
                            .SetResourceIdentifier(SamplePersons.Person)
                    },
                new object[]
                    {
                        "WithResourceAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifier
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .SetResourceIdentifier(SamplePersons.Person)
                    },
                new object[]
                    {
                        "WithResourceAndMetaAndUserBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifierWithMeta
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink)
                            .LinksEnd()
                            .ResourceIdentifier(SamplePersons.Person)
                                .SetMeta(ApiSampleData.ResourceMeta)
                            .ResourceIdentifierEnd()
                    },
                new object[]
                    {
                        "WithResourceAndMetaAndFrameworkBuiltHypermedia",
                        new ResourceIdentifierDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleToRelationshipsToAuthorLink}
                                    },
                                Data = ApiSampleData.PersonResourceIdentifierWithMeta
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleToRelationshipsToAuthorHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceIdentifier(SamplePersons.Person)
                                .SetMeta(ApiSampleData.ResourceMeta)
                            .ResourceIdentifierEnd()
                    },
            };
        #endregion


        #region DocumentBuilderWriteWithFrameworkHypermediaTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteWithFrameworkHypermediaTestData = new[]
            {
                new object[]
                    {
                        "WithResourceCollectionPath",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourcePath",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PaymentToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/payments/101"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithResourcePathAndToOneResourcePathCanonical",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101/order"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderType,
                                        Id = "1",
                                        Attributes = new ApiObject(ApiProperty.Create("total-price", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderToOrderItemsRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/orders/1/relationships/line-items"},
                                                                    {Keywords.Related, "http://api.example.com/orders/1/line-items"}
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderToPaymentsRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/orders/1/relationships/payments"},
                                                                    {Keywords.Related, "http://api.example.com/orders/1/payments"}
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderToStoreRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/orders/1/relationships/store"},
                                                                    {Keywords.Related, "http://api.example.com/orders/1/store"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/orders/1"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101/order")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrders.Order)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderToOrderItemsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToPaymentsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToStoreRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithResourcePathAndToOneResourcePathHierarchical",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/stores/50/configuration"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.StoreConfigurationType,
                                        Id = "50-Configuration",
                                        Attributes = new ApiObject(
                                            ApiProperty.Create("is-live", SampleStoreConfigurations.StoreConfiguration.IsLive),
                                            ApiProperty.Create("mailing-address", new ApiObject(
                                                ApiProperty.Create("address", SampleStoreConfigurations.StoreConfiguration.MailingAddress.Address),
                                                ApiProperty.Create("city", SampleStoreConfigurations.StoreConfiguration.MailingAddress.City),
                                                ApiProperty.Create("state", SampleStoreConfigurations.StoreConfiguration.MailingAddress.State),
                                                ApiProperty.Create("zip-code", SampleStoreConfigurations.StoreConfiguration.MailingAddress.ZipCode))),
                                            ApiProperty.Create("phone-numbers", SampleStoreConfigurations.StoreConfiguration.PhoneNumbers
                                                .Select(x =>
                                                    {
                                                        var apiObject = new ApiObject(
                                                            ApiProperty.Create("area-code", x.AreaCode),
                                                            ApiProperty.Create("number", x.Number));
                                                        return apiObject;
                                                    })
                                                .ToArray())),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.StoreToStoreConfigurationToPosSystemRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/stores/50/configuration/relationships/pos"},
                                                                    {Keywords.Related, "http://api.example.com/stores/50/configuration/pos"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/stores/50/configuration"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/stores/50/configuration")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleStoreConfigurations.StoreConfiguration)
                                .Paths()
                                    .AddPath(SampleStores.Store, ClrSampleData.StoreToStoreConfigurationRel)
                                .PathsEnd()
                                .Relationships()
                                    .AddRelationship(ClrSampleData.StoreToStoreConfigurationToPosSystemRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithResourcePathAndToOneResourcePathHierarchicalToOneResourcePathCanonical",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/stores/50/configuration/pos"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PosSystemType,
                                        Id = "RadiantRest",
                                        Attributes = new ApiObject(ApiProperty.Create("pos-system-name", SamplePosSystems.PosSystem.PosSystemName)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PosSystemToStoreConfigurationsRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/pos-systems/RadiantRest/relationships/store-configurations"},
                                                                    {Keywords.Related, "http://api.example.com/pos-systems/RadiantRest/store-configurations"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/pos-systems/RadiantRest"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/stores/50/configuration/pos")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePosSystems.PosSystem)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PosSystemToStoreConfigurationsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithResourcePathAndToManyResourceCollectionPath",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/orders/1/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/orders/1/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithResourcePathAndToManyResourceCollectionPathWithDifferentSelfAndCanonicalLinks",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/orders/1/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/orders/1/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/orders/1/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/orders/1/payments/101"},
                                                        {Keywords.Canonical, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/orders/1/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/orders/1/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/orders/1/payments/102"},
                                                        {Keywords.Canonical, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/orders/1/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Paths()
                                    .AddPath(SampleOrders.Order, ClrSampleData.OrderToPaymentsRel)
                                .PathsEnd()
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                    .AddLink(Keywords.Canonical)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithRootPathSegmentsAndResourceCollectionPath",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/api/v2/orders"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.OrderType,
                                                Id = "1",
                                                Attributes = new ApiObject(ApiProperty.Create("total-price", 100.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.OrderToOrderItemsRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/1/relationships/line-items"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items"}
                                                                        }
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderToPaymentsRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/1/relationships/payments"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/1/payments"}
                                                                        }
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderToStoreRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/1/relationships/store"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/1/store"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/api/v2/orders/1"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.OrderType,
                                                Id = "2",
                                                Attributes = new ApiObject(ApiProperty.Create("total-price", 200.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.OrderToOrderItemsRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/2/relationships/line-items"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/2/line-items"}
                                                                        }
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderToPaymentsRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/2/relationships/payments"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/2/payments"}
                                                                        }
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderToStoreRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/2/relationships/store"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/2/store"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/api/v2/orders/2"},
                                                    },
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithRootPathSegments, "http://api.example.com/api/v2/orders")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SampleOrders.Order1, SampleOrders.Order2)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderToOrderItemsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToPaymentsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToStoreRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithRootPathSegmentsAndResourcePath",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/api/v2/orders/1"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderType,
                                        Id = "1",
                                        Attributes = new ApiObject(ApiProperty.Create("total-price", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderToOrderItemsRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/orders/1/relationships/line-items"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items"}
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderToPaymentsRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/orders/1/relationships/payments"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/orders/1/payments"}
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderToStoreRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/orders/1/relationships/store"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/orders/1/store"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/api/v2/orders/1"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithRootPathSegments, "http://api.example.com/api/v2/orders/1")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrders.Order)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderToOrderItemsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToPaymentsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToStoreRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithRootPathSegmentsAndResourcePathAndToManyResourceCollectionPath",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.OrderItemType,
                                                Id = "1001",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("product-name", "Widget A"),
                                                    ApiProperty.Create("quantity", 2m),
                                                    ApiProperty.Create("unit-price", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.OrderItemToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1001/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items/1001/order"}
                                                                        }
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderItemToProductRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1001/relationships/product"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items/1001/product"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1001"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.OrderItemType,
                                                Id = "1002",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("product-name", "Widget B"),
                                                    ApiProperty.Create("quantity", 1m),
                                                    ApiProperty.Create("unit-price", 50.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.OrderItemToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1002/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items/1002/order"}
                                                                        }
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderItemToProductRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1002/relationships/product"},
                                                                            {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items/1002/product"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1002"},
                                                    },
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithRootPathSegments, "http://api.example.com/api/v2/orders/1/line-items")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SampleOrderItems.OrderItem1001, SampleOrderItems.OrderItem1002)
                                .Paths()
                                    .AddPath(SampleOrders.Order, ClrSampleData.OrderToOrderItemsRel)
                                .PathsEnd()
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderItemToOrderRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderItemToProductRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "WithRootPathSegmentsAndResourcePathAndToManyResourcePath",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1001"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderItemType,
                                        Id = "1001",
                                        Attributes = new ApiObject(
                                            ApiProperty.Create("product-name", "Widget A"),
                                            ApiProperty.Create("quantity", 2m),
                                            ApiProperty.Create("unit-price", 25.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderItemToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1001/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items/1001/order"}
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderItemToProductRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1001/relationships/product"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/orders/1/line-items/1001/product"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/api/v2/orders/1/line-items/1001"},
                                            },
                                    },
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithRootPathSegments, "http://api.example.com/api/v2/orders/1/line-items/1001")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrderItems.OrderItem1001)
                                .Paths()
                                    .AddPath(SampleOrders.Order, ClrSampleData.OrderToOrderItemsRel)
                                .PathsEnd()
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderItemToOrderRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderItemToProductRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithRootPathSegmentsAndNonResourcePathAndResourcePathAndToManyResourcePath",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/line-items/1001"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderItemType,
                                        Id = "1001",
                                        Attributes = new ApiObject(
                                            ApiProperty.Create("product-name", "Widget A"),
                                            ApiProperty.Create("quantity", 2m),
                                            ApiProperty.Create("unit-price", 25.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderItemToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/line-items/1001/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/line-items/1001/order"}
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderItemToProductRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/line-items/1001/relationships/product"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/line-items/1001/product"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/line-items/1001"},
                                            },
                                    },
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithRootPathSegments, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/line-items/1001")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrderItems.OrderItem1001)
                                .Paths()
                                    .AddPath("nrp-1")
                                    .AddPath("nrp-2")
                                    .AddPath(SampleOrders.Order, ClrSampleData.OrderToOrderItemsRel)
                                .PathsEnd()
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderItemToOrderRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderItemToProductRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithRootPathSegmentsAndNonResourcePathAndResourcePathAndNonResourcePathAndToManyResourcePath",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/nrp-3/nrp-4/line-items/1001"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderItemType,
                                        Id = "1001",
                                        Attributes = new ApiObject(
                                            ApiProperty.Create("product-name", "Widget A"),
                                            ApiProperty.Create("quantity", 2m),
                                            ApiProperty.Create("unit-price", 25.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderItemToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/nrp-3/nrp-4/line-items/1001/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/nrp-3/nrp-4/line-items/1001/order"}
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderItemToProductRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/nrp-3/nrp-4/line-items/1001/relationships/product"},
                                                                    {Keywords.Related, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/nrp-3/nrp-4/line-items/1001/product"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/nrp-3/nrp-4/line-items/1001"},
                                            },
                                    },
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithRootPathSegments, "http://api.example.com/api/v2/nrp-1/nrp-2/orders/1/nrp-3/nrp-4/line-items/1001")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrderItems.OrderItem1001)
                                .Paths()
                                    .AddPath("nrp-1")
                                    .AddPath("nrp-2")
                                    .AddPath(SampleOrders.Order, ClrSampleData.OrderToOrderItemsRel)
                                    .AddPath("nrp-3")
                                    .AddPath("nrp-4")
                                .PathsEnd()
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderItemToOrderRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderItemToProductRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "WithNonResourcePathAndResourcePathAndAllIncludedResources",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/en-us/orders/1"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderType,
                                        Id = "1",
                                        Attributes = new ApiObject(ApiProperty.Create("total-price", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderToOrderItemsRel, new ToManyRelationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/en-us/orders/1/relationships/line-items"},
                                                                    {Keywords.Related, "http://api.example.com/en-us/orders/1/line-items"}
                                                                },
                                                            Data = new List<ResourceIdentifier>
                                                                {
                                                                    new ResourceIdentifier(ClrSampleData.OrderItemType, "1001"),
                                                                    new ResourceIdentifier(ClrSampleData.OrderItemType, "1002")
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderToPaymentsRel, new ToManyRelationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/en-us/orders/1/relationships/payments"},
                                                                    {Keywords.Related, "http://api.example.com/en-us/orders/1/payments"}
                                                                },
                                                            Data = new List<ResourceIdentifier>
                                                                {
                                                                    new ResourceIdentifier(ClrSampleData.PaymentType, "101"),
                                                                    new ResourceIdentifier(ClrSampleData.PaymentType, "102")
                                                                }
                                                        }
                                                },
                                                {
                                                    ClrSampleData.OrderToStoreRel, new ToOneRelationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/en-us/orders/1/relationships/store"},
                                                                    {Keywords.Related, "http://api.example.com/en-us/orders/1/store"}
                                                                },
                                                            Data = new ResourceIdentifier(ClrSampleData.StoreType, "50")
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/en-us/orders/1"},
                                            },
                                    },
                                Included = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.OrderItemType,
                                                Id = "1001",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("product-name", "Widget A"),
                                                    ApiProperty.Create("quantity", 2m),
                                                    ApiProperty.Create("unit-price", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.OrderItemToOrderRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/orders/1/line-items/1001/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/orders/1/line-items/1001/order"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.OrderType, "1")
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderItemToProductRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/orders/1/line-items/1001/relationships/product"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/orders/1/line-items/1001/product"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.ProductType, "501")
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/orders/1/line-items/1001"},
                                                    },
                                             },
                                        new Resource
                                            {
                                                Type = ClrSampleData.OrderItemType,
                                                Id = "1002",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("product-name", "Widget B"),
                                                    ApiProperty.Create("quantity", 1m),
                                                    ApiProperty.Create("unit-price", 50.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.OrderItemToOrderRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/orders/1/line-items/1002/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/orders/1/line-items/1002/order"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.OrderType, "1")
                                                                }
                                                        },
                                                        {
                                                            ClrSampleData.OrderItemToProductRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/orders/1/line-items/1002/relationships/product"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/orders/1/line-items/1002/product"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.ProductType, "502")
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/orders/1/line-items/1002"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/payments/101/order"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.OrderType, "1")
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/payments/102/order"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.OrderType, "1")
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/payments/102"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.StoreType,
                                                Id = "50",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("store-name", "Store 50"),
                                                    ApiProperty.Create("address", "1234 Main Street"),
                                                    ApiProperty.Create("city", "Boynton Beach"),
                                                    ApiProperty.Create("state", "FL"),
                                                    ApiProperty.Create("zip-code", "33472")),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.StoreToStoreConfigurationRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/stores/50/relationships/configuration"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/stores/50/configuration"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.StoreConfigurationType, "50-Configuration")
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/stores/50"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.ProductType,
                                                Id = "501",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("name", "Widget A"),
                                                    ApiProperty.Create("unit-price", 25.0m)),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/products/501"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.ProductType,
                                                Id = "502",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("name", "Widget B"),
                                                    ApiProperty.Create("unit-price", 50.0m)),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/products/502"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.StoreConfigurationType,
                                                Id = "50-Configuration",
                                                Attributes = new ApiObject(
                                                    ApiProperty.Create("is-live", SampleStoreConfigurations.StoreConfiguration.IsLive),
                                                    ApiProperty.Create("mailing-address", new ApiObject(
                                                        ApiProperty.Create("address", SampleStoreConfigurations.StoreConfiguration.MailingAddress.Address),
                                                        ApiProperty.Create("city", SampleStoreConfigurations.StoreConfiguration.MailingAddress.City),
                                                        ApiProperty.Create("state", SampleStoreConfigurations.StoreConfiguration.MailingAddress.State),
                                                        ApiProperty.Create("zip-code", SampleStoreConfigurations.StoreConfiguration.MailingAddress.ZipCode))),
                                                    ApiProperty.Create("phone-numbers", SampleStoreConfigurations.StoreConfiguration.PhoneNumbers
                                                        .Select(x =>
                                                            {
                                                                var apiObject = new ApiObject(
                                                                    ApiProperty.Create("area-code", x.AreaCode),
                                                                    ApiProperty.Create("number", x.Number));
                                                                return apiObject;
                                                            })
                                                        .ToArray())),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.StoreToStoreConfigurationToPosSystemRel, new ToOneRelationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/stores/50/configuration/relationships/pos"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/stores/50/configuration/pos"}
                                                                        },
                                                                    Data = new ResourceIdentifier(ClrSampleData.PosSystemType, "RadiantRest")
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/stores/50/configuration"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PosSystemType,
                                                Id = "RadiantRest",
                                                Attributes = new ApiObject(ApiProperty.Create("pos-system-name", "Radiant REST-Based Api")),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PosSystemToStoreConfigurationsRelPathSegment, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/en-us/pos-systems/RadiantRest/relationships/store-configurations"},
                                                                            {Keywords.Related, "http://api.example.com/en-us/pos-systems/RadiantRest/store-configurations"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/en-us/pos-systems/RadiantRest"},
                                                    },
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/en-us/orders/1")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrders.Order)
                                .Paths()
                                    .AddPath("en-us")
                                .PathsEnd()
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderToOrderItemsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToPaymentsRel, Keywords.Self, Keywords.Related)
                                    .AddRelationship(ClrSampleData.OrderToStoreRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                            .Included()
                                .ToMany(ToManyResourceLinkage.Create(SampleOrders.Order, ClrSampleData.OrderToOrderItemsRel, SampleOrderItems.OrderItem1001, SampleOrderItems.OrderItem1002))
                                    .Paths()
                                        .AddPath("en-us")
                                        .AddPath(SampleOrders.Order, ClrSampleData.OrderToOrderItemsRel)
                                    .PathsEnd()
                                    .Relationships()
                                        .AddRelationship(ClrSampleData.OrderItemToOrderRel, Keywords.Self, Keywords.Related)
                                        .AddRelationship(ClrSampleData.OrderItemToProductRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToManyEnd()
                                .ToMany(ToManyResourceLinkage.Create(SampleOrders.Order, ClrSampleData.OrderToPaymentsRel, SamplePayments.Payment101, SamplePayments.Payment102))
                                    .Paths()
                                        .AddPath("en-us")
                                    .PathsEnd()
                                    .Relationships()
                                        .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToManyEnd()
                                .ToOne(ToOneResourceLinkage.Create(SampleOrders.Order, ClrSampleData.OrderToStoreRel, SampleStores.Store50))
                                    .Paths()
                                        .AddPath("en-us")
                                    .PathsEnd()
                                    .Relationships()
                                        .AddRelationship(ClrSampleData.StoreToStoreConfigurationRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()

                                .AddToOne(ToOneResourceLinkage.Create(SampleOrderItems.OrderItem1001, ClrSampleData.OrderItemToOrderRel, SampleOrders.Order),
                                            ToOneResourceLinkage.Create(SampleOrderItems.OrderItem1002, ClrSampleData.OrderItemToOrderRel, SampleOrders.Order))
                                .AddToOne(ToOneResourceLinkage.Create(SamplePayments.Payment101, ClrSampleData.PaymentToOrderRel, SampleOrders.Order),
                                            ToOneResourceLinkage.Create(SamplePayments.Payment102, ClrSampleData.PaymentToOrderRel, SampleOrders.Order))

                                .ToOne(ToOneResourceLinkage.Create(SampleOrderItems.OrderItem1001, ClrSampleData.OrderItemToProductRel, SampleProducts.Product501))
                                    .Paths()
                                        .AddPath("en-us")
                                    .PathsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()

                                .ToOne(ToOneResourceLinkage.Create(SampleOrderItems.OrderItem1002, ClrSampleData.OrderItemToProductRel, SampleProducts.Product502))
                                    .Paths()
                                        .AddPath("en-us")
                                    .PathsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()

                                .ToOne(ToOneResourceLinkage.Create(SampleStores.Store50, ClrSampleData.StoreToStoreConfigurationRel, SampleStoreConfigurations.Store50Configuration))
                                    .Paths()
                                        .AddPath("en-us")
                                        .AddPath<Store, long>(50, ClrSampleData.StoreToStoreConfigurationRel)
                                    .PathsEnd()
                                    .Relationships()
                                        .AddRelationship(ClrSampleData.StoreToStoreConfigurationToPosSystemRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()

                                .ToOne(ToOneResourceLinkage.Create(SampleStoreConfigurations.Store50Configuration, ClrSampleData.StoreToStoreConfigurationToPosSystemRel, SamplePosSystems.PosSystemRadiantRest))
                                    .Paths()
                                        .AddPath("en-us")
                                    .PathsEnd()
                                    .Relationships()
                                        .AddRelationship(ClrSampleData.PosSystemToStoreConfigurationsRelPathSegment, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()

                            .IncludedEnd()
                    },
                new object[]
                    {
                        "WithResourcePathAndEmptyToManyIncludedResources",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/orders/1"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderType,
                                        Id = "1",
                                        Attributes = new ApiObject(ApiProperty.Create("total-price", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderToPaymentsRel, new ToManyRelationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/orders/1/relationships/payments"},
                                                                    {Keywords.Related, "http://api.example.com/orders/1/payments"}
                                                                },
                                                            Data = new List<ResourceIdentifier>()
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/orders/1"},
                                            },
                                    },
                                Included = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/orders/1")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrders.Order)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderToPaymentsRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                            .Included()
                                .ToMany(ToManyResourceLinkage.Create(SampleOrders.Order, ClrSampleData.OrderToPaymentsRel, default(IEnumerable<Payment>)))
                                    .Relationships()
                                        .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToManyEnd()
                            .IncludedEnd()
                    },
                new object[]
                    {
                        "WithResourcePathAndNullToOneIncludedResource",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/orders/1"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.OrderType,
                                        Id = "1",
                                        Attributes = new ApiObject(ApiProperty.Create("total-price", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.OrderToStoreRel, new ToOneRelationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/orders/1/relationships/store"},
                                                                    {Keywords.Related, "http://api.example.com/orders/1/store"}
                                                                },
                                                            Data = default(ResourceIdentifier)
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/orders/1"},
                                            },
                                    },
                                Included = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/orders/1")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SampleOrders.Order)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.OrderToStoreRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                            .Included()
                                .ToOne(ToOneResourceLinkage.Create(SampleOrders.Order, ClrSampleData.OrderToStoreRel, default(Store)))
                                    .Relationships()
                                        .AddRelationship(ClrSampleData.StoreToStoreConfigurationRel, Keywords.Self, Keywords.Related)
                                    .RelationshipsEnd()
                                    .Links()
                                        .AddLink(Keywords.Self)
                                    .LinksEnd()
                                .ToOneEnd()
                            .IncludedEnd()
                    },
            };
        #endregion

        #region DocumentBuilderWriteWithPredicateBasedFrameworkHypermediaTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteWithPredicateBasedFrameworkHypermediaTestData = new[]
            {
                new object[]
                    {
                        "ResourceCollectionDocumentUsingAddRelationshipWithNoRelationshipsSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 0.0m; }, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingAddRelationshipWithSomeRelationshipsSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships(),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 50.0m; }, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingAddRelationshipWithAllRelationshipsSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships(),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships(),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 100.0m; }, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingAddRelationshipWithNoRelationshipsSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PaymentToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/payments/101"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 0.0m; }, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingAddRelationshipWithAllRelationshipsSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships(),
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/payments/101"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 100.0m; }, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingRelationshipWithNoRelationshipsSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Relationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 0.0m; })
                                        .Links()
                                            .AddLink(Keywords.Self)
                                            .AddLink(Keywords.Related)
                                        .LinksEnd()
                                    .RelationshipEnd()
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingRelationshipWithSomeRelationshipsSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships(),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Relationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 50.0m; })
                                        .Links()
                                            .AddLink(Keywords.Self)
                                            .AddLink(Keywords.Related)
                                        .LinksEnd()
                                    .RelationshipEnd()
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingRelationshipWithAllRelationshipsSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships(),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships(),
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Relationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 100.0m; })
                                        .Links()
                                            .AddLink(Keywords.Self)
                                            .AddLink(Keywords.Related)
                                        .LinksEnd()
                                    .RelationshipEnd()
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingRelationshipWithNoRelationshipsSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PaymentToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/payments/101"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Relationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 0.0m; })
                                        .Links()
                                            .AddLink(Keywords.Self)
                                            .AddLink(Keywords.Related)
                                        .LinksEnd()
                                    .RelationshipEnd()
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingRelationshipWithAllRelationshipsSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships(),
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/payments/101"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Relationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 100.0m; })
                                        .Links()
                                            .AddLink(Keywords.Self)
                                            .AddLink(Keywords.Related)
                                        .LinksEnd()
                                    .RelationshipEnd()
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingAddLinkWithNoLinksSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddLink(Keywords.Self, (payment) => { return payment.Amount > 0.0m; })
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingAddLinkWithSomeLinksSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links()
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddLink(Keywords.Self, (payment) => { return payment.Amount > 50.0m; })
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingAddLinkWithAllLinksSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links()
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links()
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddLink(Keywords.Self, (payment) => { return payment.Amount > 100.0m; })
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingAddLinkWithNoLinksSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PaymentToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/payments/101"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddLink(Keywords.Self, (payment) => { return payment.Amount > 0.0m; })
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingAddLinkWithAllLinksSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PaymentToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links(),
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddLink(Keywords.Self, (payment) => { return payment.Amount > 100.0m; })
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingLinkWithNoLinksSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/102"},
                                                    },
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Link(Keywords.Self, (payment) => { return payment.Amount > 0.0m; })
                                    .LinkEnd()
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingLinkWithSomeLinksSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links
                                                    {
                                                        {Keywords.Self, "http://api.example.com/payments/101"},
                                                    },
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links()
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Link(Keywords.Self, (payment) => { return payment.Amount > 50.0m; })
                                    .LinkEnd()
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceCollectionDocumentUsingLinkWithAllLinksSuppressed",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>
                                    {
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "101",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 75.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links()
                                            },
                                        new Resource
                                            {
                                                Type = ClrSampleData.PaymentType,
                                                Id = "102",
                                                Attributes = new ApiObject(ApiProperty.Create("amount", 25.0m)),
                                                Relationships = new Relationships
                                                    {
                                                        {
                                                            ClrSampleData.PaymentToOrderRel, new Relationship
                                                                {
                                                                    Links = new Links 
                                                                        {
                                                                            {Keywords.Self, "http://api.example.com/payments/102/relationships/order"},
                                                                            {Keywords.Related, "http://api.example.com/payments/102/order"}
                                                                        }
                                                                }
                                                        },
                                                    },
                                                Links = new Links()
                                            }
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(SamplePayments.Payment101, SamplePayments.Payment102)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Link(Keywords.Self, (payment) => { return payment.Amount > 100.0m; })
                                    .LinkEnd()
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingLinkWithNoLinksSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PaymentToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links
                                            {
                                                {Keywords.Self, "http://api.example.com/payments/101"},
                                            },
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Link(Keywords.Self, (payment) => { return payment.Amount > 0.0m; })
                                    .LinkEnd()
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "ResourceDocumentUsingLinkWithAllLinksSuppressed",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = new Resource
                                    {
                                        Type = ClrSampleData.PaymentType,
                                        Id = "101",
                                        Attributes = new ApiObject(ApiProperty.Create("amount", 100.0m)),
                                        Relationships = new Relationships
                                            {
                                                {
                                                    ClrSampleData.PaymentToOrderRel, new Relationship
                                                        {
                                                            Links = new Links 
                                                                {
                                                                    {Keywords.Self, "http://api.example.com/payments/101/relationships/order"},
                                                                    {Keywords.Related, "http://api.example.com/payments/101/order"}
                                                                }
                                                        }
                                                },
                                            },
                                        Links = new Links(),
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(SamplePayments.Payment)
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Link(Keywords.Self, (payment) => { return payment.Amount > 100.0m; })
                                    .LinkEnd()
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "EmptyResourceCollectionDocumentUsingAddRelationship",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Payment>())
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 0.0m; }, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "EmptyResourceCollectionDocumentUsingRelationship",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Payment>())
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Relationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 50.0m; })
                                        .Links()
                                            .AddLink(Keywords.Self)
                                            .AddLink(Keywords.Related)
                                        .LinksEnd()
                                    .RelationshipEnd()
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "NullResourceDocumentUsingAddRelationship",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(default(Payment))
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 0.0m; }, Keywords.Self, Keywords.Related)
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "NullResourceDocumentUsingRelationship",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(default(Payment))
                                .Relationships()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Relationship(ClrSampleData.PaymentToOrderRel, (payment) => { return payment.Amount > 0.0m; })
                                        .Links()
                                            .AddLink(Keywords.Self)
                                            .AddLink(Keywords.Related)
                                        .LinksEnd()
                                    .RelationshipEnd()
                                .RelationshipsEnd()
                                .Links()
                                    .AddLink(Keywords.Self)
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "EmptyResourceCollectionDocumentUsingAddLink",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Payment>())
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, new [] { Keywords.Self, Keywords.Related })
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddLink(Keywords.Self, (payment) => { return payment.Amount > 0.0m; })
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "EmptyResourceCollectionDocumentUsingLink",
                        new ResourceCollectionDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments"}
                                    },
                                Data = new List<Resource>()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .ResourceCollection(Enumerable.Empty<Payment>())
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, new [] { Keywords.Self, Keywords.Related })
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Link(Keywords.Self, (payment) => { return payment.Amount > 50.0m; })
                                    .LinkEnd()
                                .LinksEnd()
                            .ResourceCollectionEnd()
                    },
                new object[]
                    {
                        "NullResourceDocumentUsingAddLink",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(default(Payment))
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, new [] { Keywords.Self, Keywords.Related })
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .AddLink(Keywords.Self, (payment) => { return payment.Amount > 0.0m; })
                                .LinksEnd()
                            .ResourceEnd()
                    },
                new object[]
                    {
                        "NullResourceDocumentUsingLink",
                        new ResourceDocument
                            {
                                Links = new Links
                                    {
                                        {Keywords.Self, "http://api.example.com/payments/101"}
                                    },
                                Data = null
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithOrderResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, "http://api.example.com/payments/101")
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Resource(default(Payment))
                                .Relationships()
                                    .AddRelationship(ClrSampleData.PaymentToOrderRel, new [] { Keywords.Self, Keywords.Related })
                                .RelationshipsEnd()
                                .Links()
                                    // ReSharper disable once ConvertToLambdaExpression
                                    .Link(Keywords.Self, (payment) => { return payment.Amount > 50.0m; })
                                    .LinkEnd()
                                .LinksEnd()
                            .ResourceEnd()
                    },
            };
        #endregion


        #region DocumentBuilderWriteErrorsDocumentTestData
        public static readonly IEnumerable<object[]> DocumentBuilderWriteErrorsDocumentTestData = new[]
            {
                new object[]
                    {
                        "WithEmptyErrors",
                        ErrorsDocument.Empty,
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .Errors()
                            .ErrorsEnd()
                    },
                new object[]
                    {
                        "WithEmptyErrorsAndJsonApiAndMetaAndLinksAndUserBuiltHypermedia",
                        new ErrorsDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Errors = Enumerable.Empty<Error>()
                                                   .ToList()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleCollectionLink)
                            .LinksEnd()
                            .Errors()
                            .ErrorsEnd()
                    },
                new object[]
                    {
                        "WithEmptyErrorsAndJsonApiAndMetaAndLinksAndFrameworkBuiltHypermedia",
                        new ErrorsDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Errors = Enumerable.Empty<Error>()
                                                   .ToList()
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Errors()
                            .ErrorsEnd()
                    },
                new object[]
                    {
                        "WithReadOnlyErrors",
                        new ErrorsDocument
                            {
                                Errors = new List<Error>
                                    {
                                        ApiSampleData.Error1,
                                        ApiSampleData.Error2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .Errors()
                                .AddError(ApiSampleData.Error1)
                                .AddError(ApiSampleData.Error2)
                            .ErrorsEnd()
                    },
                new object[]
                    {
                        "WithReadOnlyErrorsAndJsonApiAndMetaAndLinksAndUserBuiltHypermedia",
                        new ErrorsDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Errors = new List<Error>
                                    {
                                        ApiSampleData.Error1,
                                        ApiSampleData.Error2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self, ApiSampleData.ArticleCollectionLink)
                            .LinksEnd()
                            .Errors()
                                .AddError(ApiSampleData.Error1, ApiSampleData.Error2)
                            .ErrorsEnd()
                    },
                new object[]
                    {
                        "WithReadOnlyErrorsAndJsonApiAndMetaAndLinksAndFrameworkBuiltHypermedia",
                        new ErrorsDocument
                            {
                                JsonApiVersion = ApiSampleData.JsonApiVersionAndMeta,
                                Meta = ApiSampleData.DocumentMeta,
                                Links = new Links
                                    {
                                        {Keywords.Self, ApiSampleData.ArticleCollectionLink}
                                    },
                                Errors = new List<Error>
                                    {
                                        ApiSampleData.Error1,
                                        ApiSampleData.Error2
                                    }
                            },
                        DocumentBuilderFactory.Create(ClrSampleData.ServiceModelWithBlogResourceTypes, HypermediaAssemblerRegistry, UrlBuilderConfigurationWithoutRootPathSegments, ApiSampleData.ArticleCollectionHRef)
                            .SetJsonApiVersion(ApiSampleData.JsonApiVersionAndMeta)
                            .SetMeta(ApiSampleData.DocumentMeta)
                            .Links()
                                .AddLink(Keywords.Self)
                            .LinksEnd()
                            .Errors()
                                .AddError(ApiSampleData.Error1, ApiSampleData.Error2)
                            .ErrorsEnd()
                    },
            };
        #endregion

        #endregion
    }
}
