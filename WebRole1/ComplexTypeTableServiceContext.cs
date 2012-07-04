using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Services.Client;
using System.Linq;
using System.Xml.Linq;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace WebRole1
{
    public class ComplexTypeTableServiceContext : TableServiceContext
    {
        public ComplexTypeTableServiceContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
            IgnoreMissingProperties = true;

            ReadingEntity += OnReadingEntity;
            WritingEntity += OnWritingEntity;
        }

        private static readonly XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private static readonly XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        private static void BindModel(object model, Type modelType, string prefix, XElement properties)
        {
            foreach (var property in modelType.GetProperties())
            {
                var propertyType = property.PropertyType;
                var propertyValue = default(object);

                if (!TypeHelpers.IsComplexType(propertyType))
                {
                    var element = properties.Element(d + prefix + "_" + property.Name);

                    if (element != null)
                    {
                        propertyValue = TypeDescriptor.GetConverter(propertyType).ConvertFrom(element.Value);
                    }
                }
                else if (TypeHelpers.IsCollection(propertyType))
                {
                    propertyValue = TypeHelpers.Create(propertyType);

                    BindCollection(propertyValue, propertyType.GetGenericArguments()[0], prefix + "_" + property.Name, properties);
                }
                else
                {
                    propertyValue = TypeHelpers.Create(propertyType);

                    BindModel(propertyValue, propertyType, prefix + "_" + property.Name, properties);
                }

                property.SetValue(model, propertyValue, null);
            }
        }

        private static void UnbindModel(object model, Type modelType, string prefix, XElement properties)
        {
            foreach (var property in modelType.GetProperties())
            {
                var propertyType = property.PropertyType;
                var propertyValue = property.GetValue(model, null);

                if (!TypeHelpers.IsComplexType(propertyType))
                {
                    var element = new XElement(d + prefix + "_" + property.Name);

                    element.SetValue(propertyValue);

                    properties.Add(element);
                }
                else if (TypeHelpers.IsCollection(propertyType))
                {
                    UnbindCollection(propertyValue, propertyType.GetGenericArguments()[0], prefix + "_" + property.Name, properties);
                }
                else
                {
                    UnbindModel(propertyValue, propertyType, prefix + "_" + property.Name, properties);
                }
            }
        }

        private static void BindCollection(object model, Type modelType, string prefix, XElement properties)
        {
            var element = properties.Element(d + prefix + "_Count");

            if (element == null || string.IsNullOrEmpty(element.Value))
            {
                return;
            }

            var count = (int)element;

            for (int i = 0; i < count; i++)
            {
                var value = default(object);

                if (!TypeHelpers.IsComplexType(modelType))
                {
                    var elem = properties.Element(d + prefix + "_" + i);

                    if (elem != null)
                    {
                        value = TypeDescriptor.GetConverter(modelType).ConvertFrom(elem.Value);
                    }
                }
                else
                {
                    value = TypeHelpers.Create(modelType);

                    BindModel(value, modelType, prefix + "_" + i, properties);
                }

                ((IList)model).Add(value);
            }
        }

        private static void UnbindCollection(object model, Type modelType, string prefix, XElement properties)
        {
            int i = 0;

            foreach (var value in (IEnumerable)model)
            {
                if (!TypeHelpers.IsComplexType(value.GetType()))
                {
                    var element = new XElement(d + prefix + "_" + i);

                    element.SetValue(value);

                    properties.Add(element);
                }
                else
                {
                    UnbindModel(value, modelType, prefix + "_" + i, properties);
                }

                i++;
            }

            properties.Add(new XElement(d + prefix + "_Count", i));
        }

        private static void OnReadingEntity(object sender, ReadingWritingEntityEventArgs e)
        {
            var properties = e.Data.Descendants(m + "properties").First();

            foreach (var property in e.Entity.GetType().GetProperties())
            {
                var propertyType = property.PropertyType;

                if (!TypeHelpers.IsComplexType(propertyType))
                {
                    continue;
                }

                var propertyValue = TypeHelpers.Create(propertyType);

                if (TypeHelpers.IsCollection(propertyType))
                {
                    BindCollection(propertyValue, propertyType.GetGenericArguments()[0], property.Name, properties);
                }
                else
                {
                    BindModel(propertyValue, propertyType, property.Name, properties);
                }

                property.SetValue(e.Entity, propertyValue, null);
            }
        }

        private static void OnWritingEntity(object sender, ReadingWritingEntityEventArgs e)
        {
            var properties = e.Data.Descendants(m + "properties").First();

            foreach (var property in e.Entity.GetType().GetProperties())
            {
                var propertyType = property.PropertyType;

                if (!TypeHelpers.IsComplexType(propertyType))
                {
                    continue;
                }

                var node = properties.Element(d + property.Name);

                if (node != null)
                {
                    node.Remove();
                }

                var propertyValue = property.GetValue(e.Entity, null);

                if (TypeHelpers.IsCollection(propertyType))
                {
                    UnbindCollection(propertyValue, propertyType.GetGenericArguments()[0], property.Name, properties);
                }
                else
                {
                    UnbindModel(propertyValue, propertyType, property.Name, properties);
                }
            }
        }
    }
}