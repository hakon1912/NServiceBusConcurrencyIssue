using IfInsurance.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Api.Infrastructure
{
    public class CommandConvention
    {
        private readonly string[] _excludedNamespaces;
        private readonly string[] _whitelistedNamespaces;

        // Namespaces known to not contain Commands even if they have names that could be matched by a convention pattern
        private readonly string[] _defaultExcludedNamespaces =
            {
            };

        // Namespaces known to only contain Commands, everything in these namespaces will be registered as commands
        private readonly string[] _defaultWhitelistedNamespaces =
            {

            };

        public CommandConvention() : this(Enumerable.Empty<string>(), Enumerable.Empty<string>())
        {
        }

        /// <summary>
        /// Use this constructor if you need to exclude or include additional namespaces to either/or
        /// excludedNamespaces or whitelistedNamespaces
        /// </summary>
        /// <param name="excludedNamespaces">Namespaces known to not contain Commands even if they have names that could be matched by a convention pattern</param>
        /// <param name="whitelistedNamespace">Namespaces known to only contain Commands, everything in these namespaces will be registered as Commands</param>
        public CommandConvention(IEnumerable<string> excludedNamespaces, IEnumerable<string> whitelistedNamespace)
        {
            _excludedNamespaces = _defaultExcludedNamespaces.Concat(excludedNamespaces).ToArray();
            _whitelistedNamespaces = _defaultExcludedNamespaces.Concat(excludedNamespaces).ToArray();
        }

        // Is the type an NServiceBus command?
        public bool IsMatch(Type t)
        {
            if (t == null || t.Namespace == null)
                return false;
            if (_excludedNamespaces.Any(x => x == t.Namespace))
                return false;
            if (!t.IsClass)
                return false;

            return
                (t.Namespace.StartsWith("IfInsurance") && MatchCommandConvention(t))
                || WhitelistedCommandNamespace(t);
        }

        private bool MatchCommandConvention(Type t)
        {
            if (t == typeof(SomeCommand))
            {

            }
            return (t.Namespace.EndsWith("Messages.Commands") // Yes
                || t.Namespace.EndsWith("MessagingContracts.Commands") // No
                || Regex.IsMatch(t.Namespace, @"Messages\..*Commands", RegexOptions.None)) // Yes
                && t.Name.EndsWith("Command"); // Yes
        }

        private bool WhitelistedCommandNamespace(Type t)
        {
            return _whitelistedNamespaces.Any(x => x.StartsWith(t.Namespace));
        }
    }
}