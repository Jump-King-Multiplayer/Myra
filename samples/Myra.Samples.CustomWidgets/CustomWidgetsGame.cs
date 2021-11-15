using System;
using System.IO;
using System.Reflection;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Assets;
using Myra.Graphics2D.UI.Properties;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D.UI.TypeResolvers;

namespace Myra.Samples.AllWidgets
{
	public class CustomWidgetsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private Desktop _desktop;

		public CustomWidgetsGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};
			Window.AllowUserResizing = true;

			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			var topPanel = (HorizontalSplitPane)Project.LoadObjectFromXml<object>(
				GetRootPanelResourceString(),
				new AssetManager(new ResourceAssetResolver(Assembly.GetExecutingAssembly(), "Resources")),
				Stylesheet.Current,
				handler: null,
				new ManualTypeResolver(typeof(Arrow))
			);

			var arrowPropertyGrid = (PropertyGrid)topPanel.EnsureWidgetById("ArrowPropertyGrid");
			arrowPropertyGrid.Object = topPanel.EnsureWidgetById("Arrow");

			topPanel.SetSplitterPosition(0, 0.75f);

			_desktop = new Desktop
			{
				Root = topPanel,

				// Inform Myra that external text input is available
				// So it stops translating Keys to chars
				HasExternalTextInput = true
			};

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

#if ANDROID
			Desktop.WidgetGotKeyboardFocus += (s, a) =>
			{
				var asTextBox = a.Data as TextBox;
				if (asTextBox == null)
				{
					return;
				}

				Guide.BeginShowKeyboardInput(PlayerIndex.One,
					"Title",
					"Description",
					asTextBox.Text,
					new AsyncCallback(r =>
					{
						var text = Guide.EndShowKeyboardInput(r);
						asTextBox.Text = text;
					}),
					null);
			};
#endif
		}

		private string GetRootPanelResourceString()
		{
			using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Myra.Samples.CustomWidgets.Resources.Root.xmmp");
			using var textReader = new StreamReader(stream ?? throw new InvalidOperationException("Could not find Root.xmmp resource"));
			return textReader.ReadToEnd();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_desktop.Render();
		}
	}
}