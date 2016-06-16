using System.Collections.Generic;
using UnityEngine;

public class UnitTests : MonoBehaviour {

    void Start() {
        ListUtilsTest luTest = new ListUtilsTest();
        luTest.test();
    }


}

public static class TestHelper {
    public static void assert(bool condition) {
        if (!condition) {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            Debug.Log("Assertion failed" + stackTrace.GetFrame(1).GetMethod().Name);
        }
    }
}

public class ListUtilsTest {

    public void test() {
        testListUtilsGetMaxAbsolutAmount1();
        testListUtilsGetMaxAbsolutAmount2();
        testListUtilsGetMaxAbsolutAmount3();

        testHighestFloat1();
    }

    private void testListUtilsGetMaxAbsolutAmount1() {
        List<object> list = new List<object> { "abc", "def", "geh" };
        TestHelper.assert(ListUtils.getMaxAbsolutAmount(list) == 2f);
    }

    private void testListUtilsGetMaxAbsolutAmount2() {
        List<object> list = new List<object> { 1f, 2.5f, 5f, 3f };
        TestHelper.assert(ListUtils.getMaxAbsolutAmount(list) == 5f);
    }

    private void testListUtilsGetMaxAbsolutAmount3() {
        List<object> list = new List<object> { 1.3f, -2f, 5f, -3f, -6.2f };
        TestHelper.assert(ListUtils.getMaxAbsolutAmount(list) == 6.2f);
    }

    private void testHighestFloat1() {
        List<object> list = new List<object> {};
        TestHelper.assert(ListUtils.getHighestFloat(list) == 0f);
    }

}
