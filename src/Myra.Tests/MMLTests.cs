using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Myra.Assets;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D.UI.TypeResolvers;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class MMLTests
	{
		[Test]
		public void LoadMMLWithExternalAssets()
		{
			var assembly = typeof(MMLTests).Assembly;
			ResourceAssetResolver assetResolver = new ResourceAssetResolver(assembly, "Resources.");
			AssetManager assetManager = new AssetManager(assetResolver);

			var mml = assetManager.Load<string>("GridWithExternalResources.xmmp");

			var project = Project.LoadFromXml(mml, assetManager);

			var imageButton1 = (ImageButton)project.Root.FindWidgetById("spawnUnit1");
			Assert.IsNotNull(imageButton1);
			Assert.IsNotNull(imageButton1.Image);
			Assert.AreEqual(imageButton1.Image.Size, new Point(64, 64));

			var label = (Label)project.Root.FindWidgetById("label");
			Assert.IsNotNull(label);
			Assert.IsNotNull(label.Font);
		}

		[Test]
		public void LoadMMLWithCustomWidgetUsingAssemblyTypeResolver()
		{
			string mml = "<CustomWidget CustomText=\"Hello\" />";
			var customWidget = (CustomWidget)Project.LoadObjectFromXml<object>(
				mml,
				assetManager: null,
				Stylesheet.Current,
				handler: null,
				new AssemblyTypeResolver(new[] { new Tuple<Assembly, string>(Assembly.GetExecutingAssembly(), "Myra.Tests") }));

			Assert.AreEqual(customWidget.CustomText, "Hello");
		}

		[Test]
		public void LoadMMLWithCustomWidgetUsingTypeResolver()
		{
			string mml = "<CustomWidget CustomText=\"Hello\" />";
			var customWidget = (CustomWidget)Project.LoadObjectFromXml<object>(
				mml,
				assetManager: null,
				Stylesheet.Current,
				handler: null,
				new ManualTypeResolver(typeof(CustomWidget))
			);

			Assert.AreEqual(customWidget.CustomText, "Hello");
		}

		[Test]
		public void LoadMMLWithWrappedCustomWidget()
		{
			string mml = "<Panel><CustomWidget Id=\"MyCustomWidget\" CustomText=\"Hello\" /></Panel>";
			var panel = (Panel)Project.LoadObjectFromXml<object>(
				mml,
				assetManager: null,
				Stylesheet.Current,
				handler: null,
				new ManualTypeResolver(typeof(CustomWidget))
			);

			var customWidget = (CustomWidget)panel.EnsureWidgetById("MyCustomWidget");

			Assert.AreEqual(customWidget.CustomText, "Hello");
		}
	}

	public class CustomWidget : Label
	{
		public string CustomText
		{
			get => Text;
			set => Text = value;
		}
	}
}
