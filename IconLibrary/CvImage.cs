﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace IconLibrary
{
	public class CvImage
	{
		Mat _Mat;

		public CvImage()
		{
		}
		private CvImage(Mat mat)
		{
			_Mat = mat;
		}

		public static CvImage Load(string path)
		{
			CvImage i = new CvImage();
			i._Mat = new Mat(path);
			return i;
		}

		public void Save(string path)
		{
			_Mat.SaveImage(path);
		}

		public CvImage Resize(int height, int width)
		{
			return new CvImage(_Mat.Resize(new Size(width, height)));
		}

		public CvImage Zoom(int limit)
		{
			int height, width;
			int x, y;
			if(_Mat.Height > _Mat.Width)
			{
				width = limit;
				height = _Mat.Height * limit / _Mat.Width;
				x = 0;
				y = (height - limit) / 2;
			}
			else
			{
				height = limit;
				width = _Mat.Width * limit / _Mat.Height;
				x = (width - limit) / 2;
				y = 0;
			}
			Mat resized = _Mat.Resize(new Size(width, height));
			Mat trimed = new Mat(resized, new Rect(x, y, limit, limit));
			return new CvImage(trimed);
		}

		#region 変換
		public unsafe LearningImage ToLearningImage()
		{
			LearningImage i = new LearningImage(_Mat.Height, _Mat.Width);
			for(int h = 0; h < _Mat.Height; h++)
			{
				long step = _Mat.Step(0);
				byte* p = _Mat.DataPointer + _Mat.Step(0) * h;
				for (int w = 0; w < _Mat.Width; w++, p += 3)
				{
					int index = (h * i.Width + w) * 3;
					i.Data[index + 0] = (double)p[0] / 255.0;
					i.Data[index + 1] = (double)p[1] / 255.0;
					i.Data[index + 2] = (double)p[2] / 255.0;
				}
			}
			return i;
		}
		#endregion
	}
}
