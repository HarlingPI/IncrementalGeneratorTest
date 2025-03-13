using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    /// <summary>
    /// 作者:   Harling
    /// 时间:   2025/3/13 16:42:59
    /// 备注:   此文件通过PIToolKit模板创建
    /// </summary>
    /// <remarks></remarks>
    [Generator]
    public class AutoToStringGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //过滤所有的类声明
            var typeinfos = context.SyntaxProvider.CreateSyntaxProvider<TypeInfo?>(
                static (s, _) => s is TypeDeclarationSyntax tds && tds.AttributeLists.Count > 0,
                static (ctx, _) =>
                {
                    var tds = (TypeDeclarationSyntax)ctx.Node;
                    var atr = tds.AttributeLists.SelectMany(a => a.Attributes).Any(a => a.Name.ToString() == "AutoToString");
                    if (!atr) return null;

                    var typeinfo = new TypeInfo()
                    {
                        Name = tds.Identifier.Text,
                        Diagnostics = new List<Diagnostic>()
                    };

                    //获取语义模型
                    var semanticModel = ctx.SemanticModel;
                    //判断是否是partial类
                    var ispartial = tds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
                    if (!ispartial)
                    {
                        // 报告诊断错误
                        typeinfo.Diagnostics.Add(Diagnostic.Create(DiagnosticRelus.NonPartialTypeRule,
                                                                   tds.Identifier.GetLocation(),
                                                                   tds.Identifier.Text,
                                                                   "AutoToString"));
                    }
                    //获取类的信息
                    var classSymbol = semanticModel.GetDeclaredSymbol(tds, _);
                    if (classSymbol.IsStatic)
                    {
                        // 报告诊断错误
                        typeinfo.Diagnostics.Add(Diagnostic.Create(DiagnosticRelus.StaticTypeRule,
                                                                   tds.Identifier.GetLocation(),
                                                                   tds.Identifier.Text,
                                                                   "AutoToString"));
                    }
                    // 获取访问修饰符
                    typeinfo.AccessModifier = Utils.GetAccessModifier(classSymbol.DeclaredAccessibility);
                    //获取类名
                    typeinfo.NamespaceName = tds.Identifier.Text;
                    //获取命名空间
                    typeinfo.NamespaceName = Utils.GetNamespaceName(tds);
                    //获取字符
                    typeinfo.Strings = tds.Members
                        .OfType<FieldDeclarationSyntax>()
                        .Select(f =>
                        {
                            //获取字段注释语法
                            var comment = Utils.GetCommentSyntax(f);

                            var variabls = f.Declaration.Variables;

                            return (comment, variabls);
                        })
                        .SelectMany(t =>
                        {
                            //当同一行声明了两个字段时直接返回这一行的字段名
                            if (t.comment == null || t.variabls.Count > 1) return t.variabls.Select(v => (v.Identifier.Text, v.Identifier.Text));
                            else
                            {
                                //获取注释内容
                                var comment = t.comment;

                                var xes = comment.ChildNodes()
                                 .OfType<XmlElementSyntax>()
                                 .FirstOrDefault(e => e.StartTag.Name.ToString() == "summary");
                                if (xes != null)
                                {
                                    //遍历注释所有节点，提取第一个非空行
                                    foreach (var xmlText in xes.Content.OfType<XmlTextSyntax>())
                                    {
                                        foreach (var token in xmlText.TextTokens)
                                        {
                                            var cleaned = token.Text.Trim().Replace("///", "");
                                            if (!string.IsNullOrEmpty(cleaned))
                                            {
                                                return [(cleaned, t.variabls[0].Identifier.Text)];
                                            }
                                        }
                                    }
                                    return [(t.variabls[0].Identifier.Text, t.variabls[0].Identifier.Text)];
                                }
                                else return t.variabls.Select(v => (v.Identifier.Text, v.Identifier.Text));
                            }
                        })
                        .Select(p => $"{p.Item1} = {{this.{p.Item2}}}")
                        .ToArray();

                    typeinfo.IsClass = tds is ClassDeclarationSyntax;

                    return typeinfo;
                });
            //生成代码
            context.RegisterSourceOutput(typeinfos,
            static (spc, info) =>
            {
                if (info == null) return;

                foreach (var diagnostic in info?.Diagnostics)
                {
                    spc.ReportDiagnostic(diagnostic);
                }
                if (info?.Diagnostics.Count > 0) return;

                var sb = new StringBuilder();

                // ======== 命名空间处理 ========
                if (!string.IsNullOrEmpty(info?.NamespaceName))
                {
                    sb.AppendLine($"namespace {info?.NamespaceName}");
                    sb.AppendLine("{");
                }

                // ======== 管理缩进 ========
                const string classIndent = "    ";
                sb.AppendLine($"{classIndent}/// <summary>");
                sb.AppendLine($"{classIndent}/// 作者:   Harling");
                sb.AppendLine($"{classIndent}/// 时间:   {DateTime.Now:yyyy/M/d hh:mm:ss}");
                sb.AppendLine($"{classIndent}/// 备注:   此文件通过代码生成器创建");
                sb.AppendLine($"{classIndent}/// </summary>");
                sb.AppendLine($"{classIndent}{info?.AccessModifier} partial {((bool)(info?.IsClass) ? "class" : "struct")} {info?.Name}");
                sb.AppendLine($"{classIndent}{{");

                const string methodIndent = "        ";
                sb.AppendLine($"{methodIndent}public override string ToString()");
                sb.AppendLine($"{methodIndent}{{");

                // ======== 字符串拼接 ========
                if (info?.Strings.Length > 0)
                {
                    sb.Append($"{methodIndent}    return $\"");
                    sb.Append(string.Join("\\n", info?.Strings));
                    sb.AppendLine("\";");
                }
                else
                {
                    sb.AppendLine($"{methodIndent}    return $\"{info?.Name} Instance (No fields)\";");
                }

                sb.AppendLine($"{methodIndent}}}");
                sb.AppendLine($"{classIndent}}}");

                if (!string.IsNullOrEmpty(info?.NamespaceName))
                {
                    sb.AppendLine("}");
                }
                // ======== 使用优化编码方式 ========
                var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);

                spc.AddSource($"{info?.Name}_AutoToString.g.cs", sourceText);
            });
        }

        private record struct TypeInfo(string NamespaceName,
                                       string AccessModifier,
                                       bool IsClass,
                                       string Name,
                                       string[] Strings,
                                       List<Diagnostic> Diagnostics);
    }
}