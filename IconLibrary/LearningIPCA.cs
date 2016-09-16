using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Analysis;
using Accord.Math;
using System.IO;

using MiniJSON;


namespace IconLibrary
{
	using Hash = Dictionary<string, object>;

	public class LearningIPCA : LearningUnit
	{
		public override LearningFrame FrameIn { get { return new LearningFrame() { Height = 150, Width = 150, Plane = 3 }; } }
		public override LearningFrame FrameOut { get { return new LearningFrame() { Height = 1, Width = 1, Plane = 16 }; } }
		public override string Filename { get { return "IPCA/"; } }

		public int MainMax
		{
			get { if (0 < TemporaryMainMax && TemporaryMainMax < FrameOut.Plane) return TemporaryMainMax; else return FrameOut.Plane; }
		}
		public int TemporaryMainMax = 0;

		public override int Scale { get { return 1; } }
		protected virtual double DynamicAmnesic
		{
			get { return 2.0 * (1 - Math.Exp(-_FrameNow / 32.0)); }	// 2.0fくらいが良い
		}

		const string FRAME_KEY = "frame";

		LearningImage[] _MainImages;	// 主成分
		LearningImage[] _TmpImages;		// 副成分

		long _FrameNow;
		public override bool IsEnoughToLearn { get { return _FrameNow > 100000; } }

		public override void Initialize()
		{
			_FrameNow = 0;
			_MainImages = new LearningImage[MainMax];
			_TmpImages = new LearningImage[MainMax];
			for (int m = 0; m < MainMax; m++)
			{
				_MainImages[m] = new LearningImage(Height, Width, Plane);
				_TmpImages[m] = new LearningImage(Height, Width, Plane);
			}
		}

		public override bool Load(string path)
		{
			Initialize();

			if (!Directory.Exists(path)) return false;
			for (int m = 0; m < MainMax; m++)
			{
				_MainImages[m] = LearningImage.LoadBin(Path.Combine(path, "main" + m + ".bin"));
				_TmpImages[m] = LearningImage.LoadBin(Path.Combine(path, "tmp" + m + ".bin"));

				if (_MainImages[m] == null || _TmpImages[m] == null) return false;
			}

			string context = File.ReadAllText(Path.Combine(path, "state.json"));
			Hash hash = Json.Deserialize(context) as Hash;
			if(hash != null)
			{
				_FrameNow = (long)hash[FRAME_KEY];
			}
			return true;
		}

		public override void Save(string path)
		{
			if (_FrameNow == 0) return;

			for(int m = 0; m < MainMax; m++)
			{
				_MainImages[m].SaveBin(Path.Combine(path, "main" + m + ".bin"));
				_TmpImages[m].SaveBin(Path.Combine(path, "tmp" + m + ".bin"));

				List<double> list = LearningImage.HighLow(_MainImages[m]);
				_MainImages[m].SavePng(Path.Combine(path, "main" + m + ".png"), list[1], list[0]);
			}

			Hash hash = new Hash();
			hash[FRAME_KEY] = _FrameNow;
			string context = Json.Serialize(hash);
			File.WriteAllText(Path.Combine(path, "state.json"), context);
		}

		public override void Learn(List<string> paths)
		{
			int limit = Height > Width ? Height : Width;
			List<LearningImage> images = new List<LearningImage>();
			foreach (string path in paths)
			{
				LearningImage image = LearningImage.LoadByZoom(path, limit);
				if (image.Height != Height || image.Width != Width) continue;
				images.Add(image);
			}
			Learn(images);
		}

		public override LearningUnit.LearningStyle Style { get { return LearningStyle.Input; } }
		public override void Learn(List<LearningImage> images)
		{
			foreach (var image in images)
			{
				if (FrameIn.Height != image.Height || FrameIn.Width != image.Width) continue;
				//if (LearningImage.EuclideanLength(image) < FrameIn.Area * 0.03) continue;
				Update(image);
			}
		}

		private void Update(LearningImage imgIn)
		{
			Array.Copy(imgIn.Data, _TmpImages[0].Data, Length);

			long iterateMax = MainMax - 1;
			if (MainMax > _FrameNow) iterateMax = _FrameNow;

			LearningImage imgA = new LearningImage(Height, Width, Plane);
			LearningImage imgB = new LearningImage(Height, Width, Plane);
			LearningImage imgC = new LearningImage(Height, Width, Plane);
			double scalerA, scalerB, scalerC;
			double nrmV;
			double l = DynamicAmnesic; //!<忘却の値、２ぐらいがよい

			for (int i = 0; i <= iterateMax; i++)
			{
				if (i == _FrameNow)
				{
					Array.Copy(_TmpImages[_FrameNow].Data, _MainImages[_FrameNow].Data, Length);
					continue;
				}

				///// Vi(n) = [a= (n-1-l)/n * Vi(n-1)] + [b= (1+l)/n * Ui(n)T Vi(n-1)/|Vi(n-1)| * Ui(n) ]
				nrmV = Norm.Euclidean(_MainImages[i].Data);

				scalerA = (double)(_FrameNow - 1 - l) / (double)_FrameNow;
				LearningImage.Sacle(_MainImages[i], imgA, scalerA);

				double dotUV = Matrix.InnerProduct(_TmpImages[i].Data, _MainImages[i].Data);
				scalerB = ((double)(1 + l) * dotUV) / ((double)_FrameNow * nrmV);
				LearningImage.Sacle(_TmpImages[i], imgB, scalerB);

				LearningImage.Add(imgA, imgB, _MainImages[i]);

				///// Ui+1(n) = Ui(n) - [c= Ui(n)T Vi(n)/|Vi(n)| * Vi(n)/|Vi(n)| ]
				if (i == iterateMax || i >= MainMax - 1) continue;

				nrmV = Norm.Euclidean(_MainImages[i].Data);
				dotUV = Matrix.InnerProduct(_TmpImages[i].Data, _MainImages[i].Data);
				scalerC = dotUV / (nrmV * nrmV);
				LearningImage.Sacle(_MainImages[i], imgC, scalerC);

				LearningImage.Sub(_TmpImages[i], imgC, _TmpImages[i + 1]);
			}
			_FrameNow++;
		}

		public override LearningImage Forecast(LearningImage image)
		{
			var results = Project(image);
			Log.Instance.Info("project: " + string.Join(",", results));
			return BackProject(results);
		}

		public override LearningImage Project(LearningImage i)
		{
			List<double> results = new List<double>();
			LearningImage amt = new LearningImage(Height, Width, Plane, i.Data);
			LearningImage tmp = new LearningImage(Height, Width, Plane);
			for (int m = 0; m < MainMax; m++)
			{
				double length = 1, result = 1;
				if (m >= 0)
				{
					length = LearningImage.EuclideanLength(_MainImages[m]);
					result = LearningImage.DotProduct(amt, _MainImages[m]) / length;
				}
				LearningImage.Sacle(_MainImages[m], tmp, result / length);
				LearningImage.Sub(amt, tmp, amt);
				results.Add(result / length);
			}
			return new LearningImage(FrameOut, results.ToArray());
		}

		public override LearningImage BackProject(LearningImage i)
		{
			List<double> list = new List<double>(i.Data);
			LearningImage image = new LearningImage(Height, Width, Plane);
			LearningImage tmp = new LearningImage(Height, Width, Plane);
			for (int m = 0; m < MainMax; m++)
			{
//				double length = LearningImage.EuclideanLength(_MainImages[m]);	本来はlist[m]*length
				LearningImage.Sacle(_MainImages[m], tmp, list[m]);
				LearningImage.Add(image, tmp, image);
			}
			return image;
		}
	}
}
