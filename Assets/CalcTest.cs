
/*#if UNITY_EDITOR
#else
using Xunit;
using Domain;
#endif

namespace Test
{
    public class CalcTest
    {
#if UNITY_EDITOR
#else
        [Fact]
#endif
        public void 足し算の結果が正しく出てくること()
        {
            int result = Calc.Sum(1,2);
            Assert.Equal(3, result);
        }
    }
}
*/