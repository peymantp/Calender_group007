// <copyright file="MenusTest.cs">Copyright ©  2016</copyright>
using NUnit.Extensions.Forms;
using NUnit.Framework;
using System.Windows.Forms;
/// <summary>
/// arthor: Peyman Justin
/// </summary>
namespace PJCalender.Tests
{
    /// <summary>This class contains parameterized unit tests for Menus</summary>
    [TestFixture]
    public partial class MenusTest 
    {
        public Menus m;

        /// <summary>
        /// Test/Scenario:
        ///     Launch calendar
        ///     
        /// Expected Result
        ///     Application starts
        /// </summary>
        [SetUp]
        public void Setup()
        {
            m = new Menus();
            m.Show();
        }
        /// <summary>
        /// Test/Scenario:
        ///     Launch another instance of the calendar
        ///     
        /// Expected Result
        ///    No new application starts. Instead, the one instance that 
        ///    was already running gets pushed to the front of the monitor.
        /// </summary>
        [Test]
        public void twoMenusTest()
        {
            Menus m2 = new Menus();
            m2.Show();
            AssertionHelper.ReferenceEquals(m, m2);
        }
        /// <summary>
        /// Test/Scenario:
        ///     Login button starts a modal window asking for username
        ///     
        /// Expected Result
        ///    A new eventDialog pops up
        /// </summary>
        [Test]
        public void loginButtonTest()
        {
            ButtonTester loginButton = new ButtonTester("buttonLog", m);
            loginButton.Click();
            int i = new FormFinder().FindAll("UsernameDialog").Count;
            Assert.AreEqual(1, i);
        }
        /// <summary>
        /// Test/Scenario:
        ///     Google asks for permissions on login
        ///     
        /// Expected Result
        ///    New tab in browser opens and requests access
        /// </summary>
        [Test]
        public void GooglePermission()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Test/Scenario:
        ///     loginButtonChangeText changes text
        ///     
        /// Expected Result
        ///     Text turns from Login to Logout
        /// </summary>
        [Test]
        public void TestLoginButtonText()
        {
            ButtonTester loginButton = new ButtonTester("buttonLog", m);
            Assert.AreEqual("Login", loginButton.Text);
            m.loginButtonChangeText();
            Assert.AreEqual("Logout", loginButton.Text);
        }
        /// <summary>
        /// Test/Scenario:
        ///     Data can be viewed in all formats (30-day, 7-day, etc..)
        ///     
        /// Expected Result
        ///     Data is correctly displayed in all viewing formats
        /// </summary>
        [Test]
        public void TabChanging()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Test/Scenario:
        ///     “Day-picker” returns the correct day.
        ///     
        /// Expected Result
        ///    View changes to show selected day
        /// </summary>
        [Test]
        public void DateTimeSelectedDefault()
        {
            System.DateTime now = System.DateTime.Now;
            string expected = now.ToLongDateString();
            LabelTester labelDay = new LabelTester("labelDay", m);
            Assert.AreEqual(expected, labelDay.Text);
        }

        [TearDown]
        public void TearDown()
        {
            m = null;
        }
    }
}
