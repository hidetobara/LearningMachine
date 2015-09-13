using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Accord.Math;

namespace IconLibrary
{
    public class LearningImage
    {
		private LearningFrame _Frame;
		public int Height { get { return _Frame.Height; } }
		public int Width { get { return _Frame.Width; } }
		public int Area { get { return _Frame.Area; } }
		public int Plane { get { return _Frame.Plane; } }
		public int Length { get { return _Frame.Length; } }
		public double[] Data;

		public LearningImage(LearningFrame f, double[] data =null)
		{
			_Frame = f;
			Data = new double[Length];
			if (data != null) Array.Copy(data, Data, Math.Min(data.Length, Length));
		}
		public LearningImage(int height, int width, int plane = 3, double[] data = null)
		{
			_Frame = new LearningFrame() { Height = height, Width = width, Plane = plane };
			Data = new double[Length];
			if (data != null) Array.Copy(data, Data, Math.Min(data.Length, Length));
		}
		public LearningImage(LearningImage image) : this(image._Frame, image.Data) { }

		public unsafe static LearningImage LoadPng(string path)
		{
			if (!File.Exists(path)) return null;
			try
			{
				Bitmap src = new Bitmap(path);
				BitmapData srcData = src.LockBits(new Rectangle(Point.Empty, src.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

				LearningImage i = new LearningImage(src.Height, src.Width);
				for (int h = 0; h < srcData.Height; h++)
				{
					byte* ps = (byte*)srcData.Scan0 + srcData.Stride * h;
					for (int w = 0; w < srcData.Width; w++, ps += i.Plane)
					{
						int postion = (i.Width * h + w) * i.Plane;
						for (int l = 0; l < i.Plane; l++)
							i.Data[postion + l] = (double)ps[l] / 255;
					}
				}
				src.UnlockBits(srcData);
				return i;
			}
			catch(Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
				return null;
			}
		}

		public LearningImage Shrink(int scale = 2)
		{
			if (scale <= 1) return this;

			LearningImage i = new LearningImage(this.Height / scale, this.Width / scale);
			for (int h = 0; h < i.Height; h++)
			{
				int hs = h * scale;
				for (int w = 0; w < i.Width; w++)
				{
					int ws = w * scale;
					List<int> list = new List<int>();
					for (int hh = hs; hh < hs + scale; hh++)
						for (int ww = ws; ww < ws + scale; ww++) list.Add(this.Width * hh + ww);
					int postion = (i.Width * h + w) * 3;
					for (int l = 0; l < Plane; l++)
						i.Data[postion + l] = Average(list, Plane, l);
				}
			}
			return i;
		}
		private double Average(List<int> list, int scale = 3, int bias = 0)
		{
			double amount = 0;
			foreach (int i in list) amount += Data[i * scale + bias];
			return amount / list.Count;
		}

		unsafe public void SavePng(string path, double low = 0, double high = 1)
		{
			Bitmap b = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
			BitmapData d = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
			for (int h = 0; h < d.Height; h++)
			{
				byte* p = (byte*)d.Scan0 + d.Stride * h;
				for (int w = 0; w < d.Width; w++, p += 3)
				{
					int position = (this.Width * h + w) * Plane;
					for (int l = 0; l < Plane && l < 3; l++)
						p[l] = Step(Data[position + l], low, high);
				}
			}
			b.UnlockBits(d);
			string dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			b.Save(path, ImageFormat.Png);
		}
		private byte Step(double v, double low = 0, double high = 1)
		{
			if (v < low) return 0;
			if (v > high) return 255;
			return (byte)((v - low) / (high - low) * 255.0);
		}

		public void SavePngAdjusted(string path)
		{
			var list = LearningImage.HighLow(this);
			SavePng(path, list[1], list[0]);
		}

		public static void Sacle(LearningImage i, LearningImage o, double scale = 1, double bias = 0)
		{
			for (int l = 0; l < o.Length; l++) o.Data[l] = i.Data[l] * scale + bias;
		}
		public static void Add(LearningImage a, LearningImage b, LearningImage o)
		{
			for (int l = 0; l < o.Length; l++) o.Data[l] = a.Data[l] + b.Data[l];
		}
		public static void Sub(LearningImage a, LearningImage b, LearningImage o)
		{
			for (int l = 0; l < o.Length; l++) o.Data[l] = a.Data[l] - b.Data[l];
		}
		public static double DotProduct(LearningImage a, LearningImage b)
		{
			return Matrix.InnerProduct(a.Data, b.Data);
		}
		public static double EuclideanLength(LearningImage a)
		{
			return Norm.Euclidean(a.Data);
		}
		public static List<double> HighLow(LearningImage a)
		{
			return new List<double>() { a.Data.Max(), a.Data.Min() };
		}

		public void SaveBin(string path)
		{
			BinaryWriter writer = null;
			try
			{
				string dir = Path.GetDirectoryName(path);
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

				List<byte> buffer = new List<byte>();
				buffer.AddRange(BitConverter.GetBytes(Height));
				buffer.AddRange(BitConverter.GetBytes(Width));
				buffer.AddRange(BitConverter.GetBytes(Plane));
				for (int l = 0; l < Length; l++) buffer.AddRange(BitConverter.GetBytes(Data[l]));
				writer = new BinaryWriter(File.OpenWrite(path));
				writer.Write(buffer.ToArray());
			}
			catch(Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
			}
			finally
			{
				if (writer != null) writer.Close();
				writer = null;
			}
		}

		public static LearningImage LoadBin(string path)
		{
			BinaryReader reader = null;
			try
			{
				if (!File.Exists(path)) return null;

				byte[] bytes = File.ReadAllBytes(path);
				int shift = 0;
				int height = BitConverter.ToInt32(bytes, shift); shift += sizeof(int);
				int width = BitConverter.ToInt32(bytes, shift); shift += sizeof(int);
				int plane = BitConverter.ToInt32(bytes, shift); shift += sizeof(int);
				LearningImage image = new LearningImage(height, width, plane);
				for (int l = 0; l < image.Length; l++) image.Data[l] = BitConverter.ToDouble(bytes, l * sizeof(double) + shift);
				return image;
			}
			catch(Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
				return null;
			}
			finally
			{
				if (reader != null) reader.Close();
				reader = null;
			}
		}

		public LearningImage Trim(Rectangle r)
		{
			LearningImage i = new LearningImage(r.Height, r.Width, Plane);
			for(int h = r.Top; h < r.Bottom; h++)
			{
				for(int w = r.Left; w < r.Right; w++)
				{
					int d = i.Width * (h - r.Top) + (w - r.Left);
					int s = h * Width + w;
					for (int p = 0; p < Plane; p++) i.Data[d * Plane + p] = this.Data[s * Plane + p];
				}
			}
			return i;
		}
		public List<LearningImage> MakeSlices(Size s)
		{
			List<LearningImage> list = new List<LearningImage>();
			for (int h = 0; h < Height - s.Height; h++)
				for(int w = 0; w < Width - s.Width; w++)
					list.Add(Trim(new Rectangle() { X = w, Y = h, Size = s }));
			return list;
		}

		public void Paste(int x, int y, LearningImage image)
		{
			for (int h = 0; h < image.Height; h++)
			{
				for (int w = 0; w < image.Width; w++)
				{
					if (h + y >= this.Height || w + x >= this.Width) continue;
					int pt = (this.Width * (h + y) + (w + x)) * Plane;
					int pi = (image.Width * h + w) * Plane;
					for (int l = 0; l < Plane; l++) this.Data[pt + l] = image.Data[pi + l];
				}
			}
		}

		public void Blend(int x, int y, LearningImage image, double rate = 0.75)
		{
			for (int h = 0; h < image.Height; h++)
			{
				for (int w = 0; w < image.Width; w++)
				{
					if (h + y >= this.Height || w + x >= this.Width) continue;
					int pt = (this.Width * (h + y) + (w + x)) * Plane;
					int pi = (image.Width * h + w) * Plane;
					for (int l = 0; l < Plane; l++) this.Data[pt + l] = this.Data[pt + l] * (1 - rate) + image.Data[pi + l] * rate;
				}
			}
		}

	}
}
