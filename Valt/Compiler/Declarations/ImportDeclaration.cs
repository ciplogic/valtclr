namespace Valt.Compiler.Declarations
{
    public class ImportDeclaration: NamedDeclaration
    {
        public override string ToString()
        {
            return "import " + Name;
        }
    }
}