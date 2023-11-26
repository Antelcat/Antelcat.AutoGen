﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Antelcat.AnyGenerator;

internal static class General
{
    internal const string Namespace = $"{nameof(Antelcat)}.{nameof(AnyGenerator)}";
    internal static string Global(Type? type) => $"global::{type?.FullName}";
    internal static string Nullable(Type? type) => type?.IsValueType == true ? string.Empty : "?";
    internal static string Generic(string? name) => name             != null ? $"<{name}>" : string.Empty;
    internal static Microsoft.CodeAnalysis.Text.SourceText SourceText(string text) =>
        Microsoft.CodeAnalysis.Text.SourceText.From(text, Encoding.UTF8);

    internal static bool IsInvalidDeclaration(this string name) => Regex.IsMatch(name, "[a-zA-Z_][a-zA-Z0-9_]*");

    internal static bool IsInvalidNamespace(this string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var parts = name.Split('.');
        return parts.All(part => part.IsInvalidDeclaration());
    }
}