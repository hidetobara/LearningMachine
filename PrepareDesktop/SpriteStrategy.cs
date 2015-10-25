using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace PrepareDesktop
{
	class SpriteStrategy
	{
		const int HeightWidth = 28;

		public void Run(string path)
		{
			string dir = Path.GetDirectoryName(path);
			string name = Path.GetFileNameWithoutExtension(path);
			string dirNew = Path.Combine(dir, name);
			if (!Directory.Exists(dirNew)) Directory.CreateDirectory(dirNew);

			Bitmap input = new Bitmap(path);
			if (input == null) return;
			List<Bitmap> outputs = Run(input);
			for(int l = 0; l < outputs.Count; l++)
			{
				string sprite = String.Format("{0:0000}", l) + ".png";
				string pathNew = Path.Combine(dirNew, sprite);
				outputs[l].Save(pathNew, ImageFormat.Png);
			}
		}

		public List<Bitmap> Run(Bitmap input)
		{
			List<Bitmap> outputs = new List<Bitmap>();
			BitmapData d = input.LockBits(new Rectangle(Point.Empty, input.Size), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			for (int h = 0; h < input.Height; h += HeightWidth)
			{
				for (int w = 0; w < input.Width; w += HeightWidth)
				{
					Bitmap sprite = MakeSprite(d, h, w);
					sprite.Palette = input.Palette;
					outputs.Add(sprite);
				}
			}
			input.UnlockBits(d);
			return outputs;
		}

		unsafe private Bitmap MakeSprite(BitmapData data, int height, int width)
		{
			Bitmap b = new Bitmap(HeightWidth, HeightWidth, PixelFormat.Format8bppIndexed);
			BitmapData d = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			for(int h = 0; h < HeightWidth; h++)
			{
				byte* pS = (byte*)(data.Scan0 + data.Stride * (height + h) + width);
				byte* pD = (byte*)(d.Scan0 + d.Stride * h);
				for (int w = 0; w < HeightWidth; w++, pS++, pD++)
				{
					pD[0] = pS[0];
				}
			}
			b.UnlockBits(d);
			return b;
		}
	}
}
