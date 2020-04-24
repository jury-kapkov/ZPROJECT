using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class FastBitmap
{
	private int stride;
	private byte[] data;

	public int Width { get; }
	public int Height { get; }

	public FastBitmap(int width, int height, Color[,] colors)
	{
		data = ConvertToByte(colors);
		Width = width;
		Height = height;
		stride = data.Length / height;
	}

	private byte[] ConvertToByte(Color[,] colors)
	{
		byte[] result = new byte[colors.Length * 4];
		int position = 0;
		for (int y = 0; y < colors.GetLength(1); ++y)
			for (int x = 0; x < colors.GetLength(0); ++x)
			{
				result[position++] = colors[x, y].B;
				result[position++] = colors[x, y].G;
				result[position++] = colors[x, y].R;
				result[position++] = colors[x, y].A;
			}
		return result;
	}

	public FastBitmap(int width, int height, byte[] data)
	{
		this.data = data;
		Width = width;
		Height = height;
		stride = data.Length / height;
	}

	public FastBitmap(Bitmap image)
	{
		BitmapData bits = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
		stride = bits.Stride;
		int bytes = stride * bits.Height;
		data = new byte[bytes];
		Marshal.Copy(bits.Scan0, data, 0, bytes);
		image.UnlockBits(bits);
		Width = image.Width;
		Height = image.Height;
	}

	public FastBitmap(int width, int height, Color color) : this(new Bitmap(width, height))
	{
		FillRectangle(0, 0, Width, Height, color);
	}

	public byte[] GetData()
	{
		return data;
	}

	public Bitmap GetBitmap()
	{
		Bitmap result = new Bitmap(Width, Height);
		BitmapData bits = result.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
		Marshal.Copy(data, 0, bits.Scan0, data.Length);
		result.UnlockBits(bits);
		return result;
	}

	public Color GetPixel(int x, int y)
	{
		int position = y * stride + x * 4;
		int b = data[position++];
		int g = data[position++];
		int r = data[position++];
		int a = data[position++];
		return Color.FromArgb(a, r, g, b);
	}

	private void SetPixel(int x, int y, byte r, byte g, byte b, byte a = 255)
	{
		if (x >= 0 && y >= 0 && x < Width && y < Height)
		{
			int position = y * stride + x * 4;
			data[position] = (byte)(b * ((float)a / 255) + data[position] * ((float)(255 - a) / 255));
			++position;
			data[position] = (byte)(g * ((float)a / 255) + data[position] * ((float)(255 - a) / 255));
			++position;
			data[position] = (byte)(r * ((float)a / 255) + data[position] * ((float)(255 - a) / 255));
			++position;
			data[position] = (byte)(a * ((float)a / 255) + data[position] * ((float)(255 - a) / 255));
		}
	}

	public void SetPixel(int x, int y, Color color)
	{
		SetPixel(x, y, color.R, color.G, color.B, color.A);
	}

	public void SwapPixels(int x, int y, int newX, int newY)
	{
		Color color = GetPixel(x, y);
		SetPixel(x, y, GetPixel(newX, newY));
		SetPixel(newX, newY, color);
	}

	public void DrawRectangle(int x, int y, int width, int height, Color color)
	{
		DrawLine(x, y, x + width - 1, y, color);
		DrawLine(x + width - 1, y, x + width - 1, y + height - 1, color);
		DrawLine(x, y + height - 1, x + width - 1, y + height - 1, color);
		DrawLine(x, y, x, y + height - 1, color);
	}

	public void DrawRectangle(Point start, Point end, Color color)
	{
		int x = Math.Min(start.X, end.X);
		int y = Math.Min(start.Y, end.Y);
		int width = Math.Abs(end.X - start.X);
		int height = Math.Abs(end.Y - start.Y);
		DrawRectangle(x, y, width, height, color);
	}

	public void DrawRectangle(Rectangle rectangle, Color color)
	{
		DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
	}

	public void FillRectangle(int x, int y, int width, int height, Color color)
	{
		for (int i = Math.Max(0, y); i < y + height && i < Height; ++i)
			for (int j = Math.Max(0, x); j < x + width && j < Width; ++j)
				SetPixel(j, i, color);
	}

	public void FillRectangle(Rectangle rectangle, Color color)
	{
		FillRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
	}

	public void FillRectangle(Point start, Point end, Color color)
	{
		int x = Math.Min(start.X, end.X);
		int y = Math.Min(start.Y, end.Y);
		int width = Math.Abs(end.X - start.X);
		int height = Math.Abs(end.Y - start.Y);
		FillRectangle(x, y, width, height, color);
	}

	public void Fill(int x, int y, Color color)
	{
		Color targetColor = GetPixel(x, y);
		if (!targetColor.ToArgb().Equals(color.ToArgb()))
		{
			Queue<Point> queue = new Queue<Point>();
			queue.Enqueue(new Point(x, y));
			while (queue.Count > 0)
			{
				Point point = queue.Dequeue();
				if (GetPixel(point.X, point.Y).ToArgb().Equals(targetColor.ToArgb()))
				{
					int w = point.X;
					int e = point.X;
					while (w > 0 && GetPixel(w - 1, point.Y).ToArgb().Equals(targetColor.ToArgb())) --w;
					while (e < Width - 1 && GetPixel(e + 1, point.Y).ToArgb().Equals(targetColor.ToArgb())) ++e;
					for (int i = w; i <= e; ++i)
					{
						SetPixel(i, point.Y, color);
						if (point.Y > 0 && GetPixel(i, point.Y - 1).ToArgb().Equals(targetColor.ToArgb())) queue.Enqueue(new Point(i, point.Y - 1));
						if (point.Y < Height - 1 && GetPixel(i, point.Y + 1).ToArgb().Equals(targetColor.ToArgb())) queue.Enqueue(new Point(i, point.Y + 1));
					}
				}

			}
		}
	}

	public void DrawLine(int startX, int startY, int endX, int endY, Color color)
	{
		bool swap = false;
		if (Math.Abs(endY - startY) > Math.Abs(endX - startX))
		{
			Swap(ref startX, ref startY);
			Swap(ref endX, ref endY);
			swap = true;
		}
		if (endX < startX)
		{
			Swap(ref startX, ref endX);
			Swap(ref startY, ref endY);
		}
		int lengthX = endX - startX;
		int doubleLengthY = Math.Abs(endY - startY) * 2;
		int doubleLengthX = lengthX * 2;
		int step = startY < endY ? 1 : -1;
		int y = startY;
		int error = 0;
		for (int x = startX; x <= endX; ++x)
		{
			if (swap) SetPixel(y, x, color);
			else SetPixel(x, y, color);
			error += doubleLengthY;
			if (error > lengthX)
			{
				y += step;
				error -= doubleLengthX;
			}
		}
	}

	public void DrawLine(Point start, Point end, Color color)
	{
		DrawLine(start.X, start.Y, end.X, end.Y, color);
	}

	public void DrawCircle(Point center, int radius, Color color)
	{
		int x = 0;
		int y = radius;
		int delta = 1 - 2 * radius;
		int error = 0;
		while (y >= 0)
		{
			SetPixel(center.X + x, center.Y + y, color);
			SetPixel(center.X + x, center.Y - y, color);
			SetPixel(center.X - x, center.Y + y, color);
			SetPixel(center.X - x, center.Y - y, color);
			error = 2 * (delta + y) - 1;
			if (delta < 0 && error <= 0)
				delta += 2 * ++x + 1;
			else
			{
				if (delta > 0 && error > 0)
					delta -= 2 * --y + 1;
				else
					delta += 2 * (++x - y--);
			}
		}
	}

	public void FillCircle(Point center, int radius, Color color)
	{
		int x = 0;
		int y = radius;
		int delta = 1 - 2 * radius;
		int error = 0;
		while (y >= 0)
		{
			for (int i = center.X - x; i <= center.X + x; ++i)
			{
				SetPixel(i, center.Y + y, color);
				SetPixel(i, center.Y - y, color);
			}
			error = 2 * (delta + y) - 1;
			if (delta < 0 && error <= 0)
				delta += 2 * ++x + 1;
			else
			{
				if (delta > 0 && error > 0)
					delta -= 2 * --y + 1;
				else
					delta += 2 * (++x - y--);
			}
		}
	}

	private void Swap(ref int a, ref int b)
	{
		int temp = a;
		a = b;
		b = temp;
	}
}