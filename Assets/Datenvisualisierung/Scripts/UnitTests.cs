using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit Tests for some heavy used and needed functions
/// </summary>
public class UnitTests : MonoBehaviour {

    /// <summary>
    /// Startup unit tests
    /// </summary>
    void Start() {
        ListUtilsTest luTest = new ListUtilsTest();
        luTest.test();
    }
}

/// <summary>
/// Helper class for the tests
/// </summary>
public static class TestHelper {

    /// <summary>
    /// simple assert function, check the boolean condition and print a debug message with the test method name if the tests failed 
    /// </summary>
    /// <param name="condition">the condition to check if true</param>
    public static void assert(bool condition) {
        if (!condition) {
            ///get stacktrace to get the test method name
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            Debug.Log("Assertion failed" + stackTrace.GetFrame(1).GetMethod().Name);
        }
    }
}

/// <summary>
/// Tests class for the ListUtil class
/// </summary>
public class ListUtilsTest {

    /// <summary>
    /// aggregate all test methods
    /// </summary>
    public void test() {
        testListUtilsGetMaxAbsolutAmount1();
        testListUtilsGetMaxAbsolutAmount2();
        testListUtilsGetMaxAbsolutAmount3();

        testHighestFloat1();
    }

    #region actual test methods

    /// <summary>
    /// Testing the getMaxAbsolutAmount method for string lists
    /// </summary>
    private void testListUtilsGetMaxAbsolutAmount1() {
        List<object> list = new List<object> { "abc", "def", "geh" };
        TestHelper.assert(ListUtils.getMaxAbsolutAmount(list) == 3f);
    }

    /// <summary>
    /// Testing the getMaxAbsolutAmount method for positive float values
    /// </summary>
    private void testListUtilsGetMaxAbsolutAmount2() {
        List<object> list = new List<object> { 1f, 2.5f, 5f, 3f };
        TestHelper.assert(ListUtils.getMaxAbsolutAmount(list) == 5f);
    }

    /// <summary>
    /// Testing the getMaxAbsolutAmount method for negative and positive float values
    /// </summary>
    private void testListUtilsGetMaxAbsolutAmount3() {
        List<object> list = new List<object> { 1.3f, -2f, 5f, -3f, -6.2f };
        TestHelper.assert(ListUtils.getMaxAbsolutAmount(list) == 6.2f);
    }

    /// <summary>
    /// Testing the getHighestFloat method for empty lists
    /// </summary>
    private void testHighestFloat1() {
        List<object> list = new List<object> {};
        TestHelper.assert(ListUtils.getHighestFloat(list) == 0f);
    }

    #endregion
}