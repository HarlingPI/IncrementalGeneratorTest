using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    /// <summary>
    /// 作者:   Harling
    /// 时间:   2025/3/13 16:44:24
    /// 备注:   此文件通过PIToolKit模板创建
    /// </summary>
    /// <remarks></remarks>
    public static class DiagnosticRelus
    {
        public static readonly DiagnosticDescriptor NonEnumTypeRule = new("SG0001",
                                                                          "非枚举类型",
                                                                          "{0} 只能是枚举类型",
                                                                          "Syntax",
                                                                          DiagnosticSeverity.Error,
                                                                          true);
        public static readonly DiagnosticDescriptor NonPartialTypeRule = new("SG0002",
                                                                              "非部分类或结构体",
                                                                              "{0} 只能用于部分类或部分结构体",
                                                                              "Syntax",
                                                                              DiagnosticSeverity.Error,
                                                                              true);
        public static readonly DiagnosticDescriptor StaticTypeRule = new("SG0003",
                                                                          "静态部分类或结构体",
                                                                          "{0} 只能用于非静态部分类或部分结构体",
                                                                          "Syntax",
                                                                          DiagnosticSeverity.Error,
                                                                          true);

        public static readonly DiagnosticDescriptor DuplicateEnumValueRule = new("SG0004",
                                                                                  "重复的枚举值注册",
                                                                                  "枚举值 '{0}.{1}' 被多个转换器重复注册（冲突类型：{2}）",
                                                                                  "Usage",
                                                                                  DiagnosticSeverity.Error,
                                                                                  true);

        public static readonly DiagnosticDescriptor AutoObjectRule = new("SG0005",
                                                                          "没有目标标记",
                                                                          "AutoConverter 标记的转换器目标类必须含有AutoObject标记",
                                                                          "Syntax",
                                                                          DiagnosticSeverity.Error,
                                                                          true);


    }
}