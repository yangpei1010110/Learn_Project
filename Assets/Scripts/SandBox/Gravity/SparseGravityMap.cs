#nullable enable
namespace SandBox.Gravity
{
    public class SparseGravityMap
    {
        #region Instance

        private static SparseGravityMap? _instance;
        public static  SparseGravityMap  Instance => _instance ??= new SparseGravityMap();

        #endregion
    }
}