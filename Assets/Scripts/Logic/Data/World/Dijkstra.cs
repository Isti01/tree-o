namespace Logic.Data.World {
public class Dijkstra {
	public int D { get; set; }
	public int Ox { get; set; }
	public int Oy { get; set; }
	public int Px { get; set; }
	public int Py { get; set; }

	public Dijkstra(int x, int y) {
		D = int.MaxValue;
		Ox = x;
		Oy = y;
		Px = -1;
		Py = -1;
	}
}
}
