using SandBox.Map;

namespace SandBox.Elements
{
    public static class ElementSimulation
    {
        public static void Run(ref MapBlock mapBlock, ref IElement element)
        {
            switch (element.Type)
            {
                case ElementType.Solid:
                    SolidSimulation(ref mapBlock, ref element);
                    break;
                case ElementType.Liquid:
                    LiquidSimulation(ref mapBlock, ref element);
                    break;
                case ElementType.Gas:
                    GasSimulation(ref mapBlock, ref element);
                    break;
            }
        }

        public static void SolidSimulation(ref MapBlock mapBlock, ref IElement element)
        {
        }

        public static void LiquidSimulation(ref MapBlock mapBlock, ref IElement element)
        {
        }

        public static void GasSimulation(ref MapBlock mapBlock, ref IElement element)
        {
        }
    }
}