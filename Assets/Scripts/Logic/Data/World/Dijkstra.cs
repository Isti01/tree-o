namespace Logic.Data.World {
internal class Dijkstra {
	public int D { get; set; }
	public int Ox { get; }
	public int Oy { get; }
	public int Px { get; set; }
	public int Py { get; set; }
	public bool Queued { get; set; }

	public Dijkstra(int x, int y) {
		D = int.MaxValue;
		Ox = x;
		Oy = y;
		Px = -1;
		Py = -1;
	}
}
}
