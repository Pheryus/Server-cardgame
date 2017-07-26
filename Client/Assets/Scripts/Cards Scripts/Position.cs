

public class Position {

	public int column, line;
    public int side;


	public Position(int line, int column, int side = -1) {
		this.column = column;
		this.line = line;
        this.side = side;
	}

}