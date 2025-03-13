using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    /// <summary>
    /// 作者:   Harling
    /// 时间:   2025/3/13 16:43:39
    /// 备注:   此文件通过PIToolKit模板创建
    /// </summary>
    /// <remarks></remarks>
    internal class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetEnumMemberName(INamedTypeSymbol enumType, int value)
        {
            return enumType.GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(f => f.ConstantValue?.Equals(value) ?? false)?
                .Name ?? $"Unknown_{value}";
        }
        /// <summary>
        /// 获取类型全名（包含命名空间）
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTypeName(ISymbol symbol)
        {
            //使用全称
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                        // 移除 global::
                        .Replace("global::", "");
        }
        public static string GetAccessModifier(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                Accessibility.ProtectedAndInternal => "private protected",
                _ => "internal"
            };
        }

        public static DocumentationCommentTriviaSyntax? GetCommentSyntax(FieldDeclarationSyntax field)
        {
            return field.GetLeadingTrivia()
                              .Select(t => t.GetStructure())
                              .OfType<DocumentationCommentTriviaSyntax>()
                              .FirstOrDefault();
        }

        public static string GetNamespaceName(TypeDeclarationSyntax tds)
        {
            var namespaceNode = tds.Parent;
            while (namespaceNode != null)
            {
                if (namespaceNode is NamespaceDeclarationSyntax nds)
                {
                    return nds.Name.ToString();
                }
                namespaceNode = namespaceNode.Parent;
            }
            return null;
        }
    }
}