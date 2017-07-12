using System.Collections;

public class Targets {

    public Card[,] field;

    public Targets() {
        field = new Card[3, 2];
    }

    public Card[] GetAllAvaliableCharacters() {

        Card[] targets = new Card[40];
        Card card;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 3; j++) {
                card = field[i, j];

                //se a carta não tiver imunidade ou não estiver furtiva, pode ser alvo
                if (!card.isImmuny() || !card.isInvisible()) {
                    //can be target
                }
            }
        }
        return targets;
    }

    public ArrayList GetAllLineCharacters(int line) {

        ArrayList targets = new ArrayList();

        for (int i = 0; i < 3; i++) {
            Card card = field[line, i];
            //se a carta não tiver imunidade ou não estiver furtiva, pode ser alvo
            if (!card.isImmuny() || !card.isInvisible()) {
                //can be target
                targets.Add(card);
            }
        }
        return targets;
    }


    public ArrayList GetAllColumnCharacters(int column) {

        ArrayList targets = new ArrayList();

        for (int i = 0; i < 3; i++) {
            Card card = field[i, column];
            //se a carta não tiver imunidade ou não estiver furtiva, pode ser alvo
            if (!card.isImmuny() || !card.isInvisible()) {
                //can be target
                targets.Add(card);
            }
        }
        return targets;
    }
}