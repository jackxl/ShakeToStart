using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ShakeToStart.Model;
using Windows.UI.Xaml.Controls;

namespace UnitTestApp
{
    [TestClass]
    public class UnitTestUri
    {
        [TestMethod]
        public void TestSerializeAndDeserializeName()
        {
            UriItem uriItem = new UriItem();
            uriItem.name = "test";
            uriItem.uri = new Uri("http://www.test.com");
            uriItem.symbol = (Symbol)10;

            string serializedUriItem = uriItem.GetSerializedUriObject();

            UriItem newUriItem = UriItem.DesirializeUriString(serializedUriItem);

            Assert.AreEqual<string>(uriItem.name, newUriItem.name);
        }

        [TestMethod]
        public void TestSerializeAndDeserializeUriLocal()
        {
            UriItem uriItem = new UriItem();
            uriItem.name = "test";
            uriItem.uri = new Uri("ms-settings:");
            uriItem.symbol = (Symbol)10;

            string serializedUriItem = uriItem.GetSerializedUriObject();

            UriItem newUriItem = UriItem.DesirializeUriString(serializedUriItem);
            
            Assert.AreEqual<Uri>(uriItem.uri, newUriItem.uri);
        }

        [TestMethod]
        public void TestSerializeAndDeserializeUriExternal()
        {
            UriItem uriItem = new UriItem();
            uriItem.name = "test";
            uriItem.uri = new Uri("http://www.google.com");
            uriItem.symbol = (Symbol)10;

            string serializedUriItem = uriItem.GetSerializedUriObject();

            UriItem newUriItem = UriItem.DesirializeUriString(serializedUriItem);

            Assert.AreEqual<Uri>(uriItem.uri, newUriItem.uri);
        }

        [TestMethod]
        public void TestSerializeAndDeserializeSymbol()
        {
            UriItem uriItem = new UriItem();
            uriItem.name = "test";
            uriItem.uri = new Uri("http://www.test.com");
            uriItem.symbol = (Symbol)10;

            string serializedUriItem = uriItem.GetSerializedUriObject();

            UriItem newUriItem = UriItem.DesirializeUriString(serializedUriItem);

            Assert.AreEqual<Symbol>(uriItem.symbol, newUriItem.symbol);
        }
    }
}
