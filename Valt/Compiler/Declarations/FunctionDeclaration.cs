using System.Collections.Generic;
using Valt.Compiler.PrePass;
using Valt.Compiler.Typing;

namespace Valt.Compiler.Declarations
{
    public class ParameterDeclaration : NamedDeclaration
    {
        public ResolvedType ArgType { get; set; }
    }
    public class FunctionDeclaration : NamedDeclaration
    {
        public ResolvedType ReturnType { get; set; }
        public bool IsExtension { get; set; }
        public List<ParameterDeclaration> Args { get; } = new List<ParameterDeclaration>();

        public static FunctionDeclaration DeclarationEvaluation(PreModuleDeclaration funcDecl)
        {
            //TODO do argument handling and so on
            FunctionDeclaration result = new FunctionDeclaration();
            return result;
        }
    }
}