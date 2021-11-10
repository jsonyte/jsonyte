using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Text;

namespace Jsonyte
{
    /*
        TODO:


        include
        fields
        sort
        paging
        filtering


        =========================

        GET /articles/1?include=author,comments.author

        GET /articles?fields[articles]=title,body&fields[people]=name

        GET /people?sort=age
        GET /articles?sort=-created,title

        Paging (needs strategy)
        page[number]
        page[size]
        -- OR
        page[offset]
        page[limit]
        -- OR
        page[cursor]

        Filtering (needs strategy)
        filter[name]=something
        -- OR
        filter[name]=eq:something, filter[age]=lt:25
        -- OR
        filter=equals(lastname, 'Smith'), filter=not(equals(lastname, 'smith'))
     */

    /// <summary>
    /// Provides a fluent builder for creating <see href="https://jsonapi.org/">JSON:API</see> uniform resource identifiers (URIs).
    /// </summary>
    public class JsonApiUriBuilder : UriBuilder
    {
        private static readonly ConcurrentDictionary<Type, string> TypeNames = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class.
        /// </summary>
        public JsonApiUriBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the specified URI.
        /// </summary>
        /// <param name="uri">A URI string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
        /// <exception cref="UriFormatException"><paramref name="uri"/> is not a valid URI.</exception>
        public JsonApiUriBuilder(string uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the scheme and host.
        /// </summary>
        /// <param name="schemeName">An Internet access protocol.</param>
        /// <param name="hostName">A DNS-style domain name or IP address.</param>
        public JsonApiUriBuilder(string schemeName, string hostName)
            : base(schemeName, hostName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the scheme, host and port.
        /// </summary>
        /// <param name="scheme">An Internet access protocol.</param>
        /// <param name="host">A DNS-style domain name or IP address.</param>
        /// <param name="portNumber">An IP port number for the service.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="portNumber" /> is less than -1 or greater than 65,535.</exception>
        public JsonApiUriBuilder(string scheme, string host, int portNumber)
            : base(scheme, host, portNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the scheme, host, port and path.
        /// </summary>
        /// <param name="scheme">An Internet access protocol.</param>
        /// <param name="host">A DNS-style domain name or IP address.</param>
        /// <param name="port">An IP port number for the service.</param>
        /// <param name="pathValue">The path to the Internet resource.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port" /> is less than -1 or greater than 65,535.</exception>
        public JsonApiUriBuilder(string scheme, string host, int port, string pathValue)
            : base(scheme, host, port, pathValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the specified <see cref="Uri" /> instance.
        /// </summary>
        /// <param name="uri">An instance of the <see cref="Uri" /> class.</param>
        /// <exception cref="ArgumentNullException"><paramref name="uri" /> is <see langword="null" />.</exception>
        public JsonApiUriBuilder(Uri uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Gets or sets the naming policy used to convert property names for includes, sparse fields, sorting and filtering.
        /// 
        /// Default value is camel-casing.
        /// </summary>
        /// <returns>A property naming policy, or <see langword="null" /> to leave property names unchanged. </returns>
        public JsonNamingPolicy? NamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;

        /// <summary>
        /// Gets a read-only instance of a <see cref="NameValueCollection"/> that contains the keys and values of the query string.
        /// </summary>
        /// <returns>A read-only instance containing the keys and values of the query string.</returns>
        public NameValueCollection GetQueryParameters()
        {
            return QueryParametersCollection.Parse(Query);
        }

        /// <summary>
        /// Add a query parameter and optional values to the query.
        /// </summary>
        /// <param name="key">The query parameter to add to the query.</param>
        /// <param name="values">The optional query parameter values to add to the query.</param>
        /// <returns></returns>
        public JsonApiUriBuilder AddQuery(string key, params string[] values)
        {
            var parameters = QueryParametersCollection.Parse(Query);

            if (values.Length == 0)
            {
                parameters.Add(null, key);
            }
            else
            {
                foreach (var value in values)
                {
                    parameters.Add(key, value);
                }
            }

            Query = parameters.ToString();

            return this;
        }

        /// <summary>
        /// Add an included relationship parameter to the query.
        /// </summary>
        /// <param name="relationship">The name of the relationship.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder Include(string relationship)
        {
            return AddQuery("include", ConvertName(relationship));
        }

        /// <summary>
        /// Add a sort parameter to the query.
        /// </summary>
        /// <param name="member">The name of the attribute to sort by.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder OrderBy(string member)
        {
            return AddQuery("sort", ConvertName(member));
        }

        /// <summary>
        /// Add a descending sort parameter to the query.
        /// </summary>
        /// <param name="member">The name of the attribute to sort descending by.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder OrderByDescending(string member)
        {
            return AddQuery("sort", $"-{ConvertName(member)}");
        }

        /// <summary>
        /// Add a sparse field parameter to the query for a given resource type.
        /// </summary>
        /// <param name="type">The type of the resource.</param>
        /// <param name="member">The attribute in the resource to include.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder IncludeField(string type, string member)
        {
            return IncludeFields(type, member);
        }

        /// <summary>
        /// Add multiple sparse field parameters to the query for a given resource type.
        /// </summary>
        /// <param name="type">The type of the resource.</param>
        /// <param name="members">The attributes in the resource to include.</param>
        /// <returns>The query builder instance</returns>
        public JsonApiUriBuilder IncludeFields(string type, params string[] members)
        {
            AddQuery($"fields[{type}]", members.Select(ConvertName).ToArray());

            return this;
        }

        internal string ConvertName(string name)
        {
            return NamingPolicy?.ConvertName(name) ?? name;
        }

        internal string GetTypeName(Type type, string? name = null)
        {
            // 1. Use name passed in first
            if (!string.IsNullOrEmpty(name))
            {
                return name!;
            }

            return TypeNames.GetOrAdd(type, x =>
            {
                // 2. Try and create the object and use the Type property
                if (x.IsResource())
                {
                    var member = x.GetTypeMember();
                    var constructor = x.GetConstructor(Type.EmptyTypes);

                    if (constructor != null)
                    {
                        var resource = constructor.Invoke(null);

                        var value = member switch
                        {
                            PropertyInfo property => property.GetValue(resource)?.ToString(),
                            FieldInfo field => field.GetValue(resource)?.ToString(),
                            _ => null
                        };

                        if (!string.IsNullOrEmpty(value))
                        {
                            TypeNames[x] = value!;

                            return value!;
                        }
                    }
                }

                // 3. Use the type name and make a best guess
                var typeName = ConvertName(x.Name);

                return Pluralizer.Pluralize(typeName);
            });
        }
    }

    /// <summary>
    /// Provides a fluent builder for creating <see href="https://jsonapi.org/">JSON:API</see> uniform resource identifiers (URIs)
    /// using the specified <typeparamref name="T"/> resource type.
    /// </summary>
    /// <typeparam name="T">The JSON:API resource type.</typeparam>
    public class JsonApiUriBuilder<T> : JsonApiUriBuilder
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Dictionary<string, List<string>> excludedFields = new();

        /// <inheritdoc />
        public JsonApiUriBuilder()
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string uri)
            : base(uri)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string schemeName, string hostName)
            : base(schemeName, hostName)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string scheme, string host, int portNumber)
            : base(scheme, host, portNumber)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string scheme, string host, int port, string pathValue)
            : base(scheme, host, port, pathValue)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(Uri uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Add an included relationship parameter to the query using an expression.
        /// </summary>
        /// <typeparam name="TRelationship">The type of the resource.</typeparam>
        /// <param name="expression">The expression path to a relationship to include.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder<T> Include<TRelationship>(Expression<Func<T, TRelationship>> expression)
        {
            Include(GetMemberPath(expression));

            return this;
        }

        /// <summary>
        /// Add a sort parameter to the query using an expression.
        /// </summary>
        /// <typeparam name="TMember">The type of the resource.</typeparam>
        /// <param name="expression">The expression path to an attribute to sort by.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder<T> OrderBy<TMember>(Expression<Func<T, TMember>> expression)
        {
            OrderBy(GetMemberPath(expression));

            return this;
        }

        /// <summary>
        /// Add a descending sort parameter to the query using an expression.
        /// </summary>
        /// <typeparam name="TMember">The type of the resource.</typeparam>
        /// <param name="expression">The expression path to an attribute to sort descending by.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder<T> OrderByDescending<TMember>(Expression<Func<T, TMember>> expression)
        {
            OrderByDescending(GetMemberPath(expression));

            return this;
        }

        /// <summary>
        /// Add sparse field parameters for all attributes of a given resource type.
        /// </summary>
        /// <param name="filter">An optional filter to exclude certain fields or properties.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder<T> IncludeAllFields(Func<Type, string, bool>? filter = null)
        {
            var type = typeof(T);

            var processed = new List<Type>();

            IncludeFields(type, processed, filter);

            return this;
        }

        public JsonApiUriBuilder<T> FilterBy<TMember>(Expression<Func<T, TMember>> expression)
        {

        }

        private void IncludeFields(Type type, List<Type> processed, Func<Type, string, bool>? filter)
        {
            if (processed.Contains(type))
            {
                return;
            }

            var typeName = GetTypeName(type);

            var properties = type
                .GetProperties(Flags)
                .Where(x => !x.GetIndexParameters().Any())
                .Where(x => x.GetMethod?.IsPublic == true || x.SetMethod?.IsPublic == true)
                .ToArray();

            var typeFields = type
                .GetFields(Flags)
                .Where(x => x.IsPublic)
                .ToArray();

            var members = properties.Cast<MemberInfo>()
                .Concat(typeFields);

            var eligibleMembers = members
                .Where(x => GetIgnoreCondition(x) != JsonIgnoreCondition.Always)
                .Where(x => filter == null || !filter(type, x.Name))
                .ToArray();

            var includeMembers = eligibleMembers
                .Select(GetMemberName)
                .ToArray();

            var relationshipTypes = properties
                .Select(x => x.PropertyType)
                .Concat(typeFields.Select(x => x.FieldType))
                .Where(x => x.IsResourceIdentifier());

            IncludeFields(typeName, includeMembers);

            foreach (var relationshipType in relationshipTypes)
            {
                IncludeFields(relationshipType, processed, filter);
            }
        }

        private JsonIgnoreCondition? GetIgnoreCondition(MemberInfo member)
        {
            return member.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition;
        }

        /// <summary>
        /// Add a sparse field parameter to the query for a given resource type using an expression.
        /// </summary>
        /// <remarks>
        /// The type of the resource to include is discovered by the query builder, in order, using the below strategies:
        ///
        /// <list type="number">
        ///   <item>
        ///     <description>Use the optional 'type' argument from the method.</description>
        ///   </item>
        ///   <item>
        ///     <description>Try and create an instance of the type and use the 'type' string property or field, if it exists.</description>
        ///   </item>
        ///   <item>
        ///     <description>Use the name of the type from reflection and apply a rudimentary pluralization strategy, eg. 'BankAccount' becomes 'bankAccounts'.</description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <typeparam name="TMember">The type of the property or field to include.</typeparam>
        /// <param name="expression">The expression path to the attribute to include as a sparse field.</param>
        /// <param name="type">An optional string representing the type of the resource.</param>
        /// <returns>The query builder instance.</returns>
        public JsonApiUriBuilder<T> IncludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            var (typeName, field) = GetMemberTypeAndName(expression, type);

            AddQuery($"fields[{typeName}]", field);

            return this;
        }

        /// <summary>
        /// Exclude a sparse field from the query for a given resource type using an expression.
        /// </summary>
        /// <remarks>
        /// This method is useful is <see cref="IncludeAllFields"/> was called, but where one or two attributes should be excluded.
        ///
        /// The type of the resources to include is discovered by the query builder, in order, using the below strategies:
        ///
        /// <list type="number">
        ///   <item>
        ///     <description>Use the optional 'type' argument from the method.</description>
        ///   </item>
        ///   <item>
        ///     <description>Try and create an instance of the type and use the 'type' string property or field, if it exists.</description>
        ///   </item>
        ///   <item>
        ///     <description>Use the name of the type from reflection and apply a rudimentary pluralization strategy, eg. 'BankAccount' becomes 'bankAccounts'.</description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonApiUriBuilder<T> ExcludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            AddIncludedField(expression, excludedFields, type);

            return this;
        }

        private (string type, string field) GetMemberTypeAndName<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            if (expression.Body is not MemberExpression member)
            {
                throw new JsonApiException($"Cannot parse expression: {expression}");
            }

            var field = GetMemberName(member.Member);
            var typeName = GetTypeName(member.Member.DeclaringType!, type);

            return (typeName, field);
        }

        private void AddIncludedField<TMember>(Expression<Func<T, TMember>> expression, Dictionary<string, List<string>> fields, string? type = null)
        {
            var member = expression.Body as MemberExpression;

            if (member == null)
            {
                throw new JsonApiException($"Cannot parse expression: {expression}");
            }

            var field = GetMemberName(member.Member);
            var typeName = GetTypeName(member.Member.DeclaringType!, type);

            if (!fields.TryGetValue(typeName, out var values))
            {
                fields[typeName] = values = new List<string>();
            }

            if (!values.Contains(field))
            {
                values.Add(field);
            }
        }

        private string GetMemberPath<TMember>(Expression<Func<T, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var values = new List<string>();

            while (memberExpression != null)
            {
                values.Add(GetMemberName(memberExpression.Member));

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            values.Reverse();

            return string.Join(".", values);
        }

        private string GetMemberName(MemberInfo member)
        {
            var nameAttribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();

            return nameAttribute != null
                ? nameAttribute.Name
                : ConvertName(member.Name);
        }
    }
}
