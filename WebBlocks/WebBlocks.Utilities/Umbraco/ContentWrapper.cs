﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Core.Services;
using Umbraco.Core.Composing;

namespace WebBlocks.Utilities.Umbraco
{
    public class ContentWrapper : DynamicObject, IPublishedContent
    {
        protected IContent content;
        protected ContentWrapper parent = null;
        

        public ContentWrapper(IContent content)
        {
            this.content = content;
            var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, content.ContentType.Alias);
            this._properties = content.Properties.Select(
                p => new Tuple<string, IPublishedProperty>(p.Alias,
                    new ContentPropertyWrapper(publishedContentType.PropertyTypes.FirstOrDefault(pt => pt.PropertyTypeAlias == p.PropertyType.Alias), p.Value, true)))
                .ToDictionary(c => c.Item1, c => c.Item2);
        }

        public IEnumerable<IPublishedContent> Children
        {
            get
            {
                List<IPublishedContent> children = new List<IPublishedContent>();
                foreach (Content c in content.Children())
                    children.Add(new ContentWrapper(c));
                return children;
            }
        }

        public DateTime CreateDate
        {
            get { return content.CreateDate; }
        }

        public int CreatorId
        {
            get { return content.CreatorId; }
        }

        public string CreatorName
        {
            get
            {
                //TOTEST - is this going to have a huge performance impact?
                var user = Current.Services.UserService.GetUserById(content.CreatorId);
                return user != null ? user.Name : "";
            }
        }

        public string DocumentTypeAlias
        {
            get { return content.ContentType.Alias; }
        }

        public int DocumentTypeId
        {
            get { return content.ContentType.Id; }
        }

        public int Id
        {
            get { return content.Id; }
        }

        public PublishedItemType ItemType
        {
            get { return PublishedItemType.Content; }
        }

        public int Level
        {
            get { return content.Level; }
        }

        public string Name
        {
            get { return content.Name; }
        }

        public IPublishedContent Parent
        {
            get
            {
                if (parent == null)
                {
                    var contentParent = content.Parent();
                    if (contentParent != null)
                        parent = new ContentWrapper(content.Parent());
                    else
                        return null;
                }
                return parent;
            }
        }

        public string Path
        {
            get { return content.Path; }
        }

        public IEnumerable<IPublishedProperty> Properties
        {
            get
            {
                return _properties.Values;
            }
        }

        protected Dictionary<string, IPublishedProperty> _properties;

        public int SortOrder
        {
            get { return content.SortOrder; }
        }

        public int TemplateId
        {
            get { return content.Template.Id; }
        }

        public DateTime UpdateDate
        {
            get { return content.UpdateDate; }
        }

        public string Url
        {
            get { return umbraco.library.NiceUrl(content.Id) ?? ""; }
        }

        public string UrlName
        {
            get { return content.Name.Replace(" ", "-"); }
        }

        public Guid Version
        {
            get { return content.Version; }
        }

        public int WriterId
        {
            get { return content.WriterId; }
        }

        public string WriterName
        {
            get { return content.WriterId.ToString(); }
        }

        public object this[string propertyAlias]
        {
            get { return content.Properties.FirstOrDefault(p => p.Alias == propertyAlias).Value; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!base.TryGetMember(binder, out result))
            {
                Property property = content.Properties.FirstOrDefault(p => p.Alias == binder.Name);
                result = property != null ? property.Value : "";
            }
            return true;
        }


        public IEnumerable<IPublishedContent> ContentSet
        {
            get { return new List<IPublishedContent> { this }; }
        }

        public global::Umbraco.Core.Models.PublishedContent.PublishedContentType ContentType
        {
            get { return PublishedContentType.Get(PublishedItemType.Content, content.ContentType.Alias); }
        }

        public int GetIndex()
        {
            return content.Id;
        }

       

        public bool IsDraft
        {
            get { return true; }
        }

        public string UrlSegment => throw new NotImplementedException();

        int? IPublishedContent.TemplateId => throw new NotImplementedException();

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => throw new NotImplementedException();

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => throw new NotImplementedException();

        IPublishedContentType IPublishedElement.ContentType => throw new NotImplementedException();

        public Guid Key
        {
            get { return content.Key; }
        }


        public IPublishedProperty GetProperty(string alias)
        {
            IPublishedProperty property;

            return _properties.TryGetValue(alias, out property) ? property : null;
        }

        public IPublishedProperty GetProperty(string alias, bool recurse)
        {
            var value = GetProperty(alias);
            if (!recurse)
                return value;

            if (Parent != null && Level > -1 && value == null)
                return Parent.GetProperty(alias, true);
           
            return value;
        }

        bool IPublishedContent.IsDraft(string culture)
        {
            throw new NotImplementedException();
        }

        public bool IsPublished(string culture = null)
        {
            throw new NotImplementedException();
        }
    }
}