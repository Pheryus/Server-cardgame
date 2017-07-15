using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JSONWriter {

    /*
	"playspell" : {
		"id" : magic_id,
		"position" : {
			"line" : line,
			"column" : column
		}
	}
	*/

    /*
	Padrão JSON de ataque

	"attack" : {
		"attacker_id" : attacker_id,
		"target_id" : target_id,
		"attacker_position" : {
            "side" : side,
			"line" : line,
			"column" : column
		},
		"target_position" : {
			"line" : line,
			"column" : column
		}	
	}

	*/



    //cria json para a magia	
    static public JSONObject tryPlayMagic(int magic_id) {
        JSONObject json = new JSONObject();
        JSONObject id_magic = new JSONObject();
        id_magic.AddField("id", magic_id);
        json.AddField("playspell", id_magic);
        return json;
    }

    //cria json para o alvo
    public static JSONObject setTarget(Position pos) {
        JSONObject target = new JSONObject();
        target.AddField("line", pos.line);
        target.AddField("column", pos.column);
        target.AddField("side", pos.side);
        return target;
    }



    public static JSONObject playCharacterJSON(Card card, int playerid, int hand_index, Position pos) {
        int id = card.getID();
        JSONObject cardJSON = new JSONObject();
        JSONObject json = new JSONObject();
        json.AddField("id", id);
        json.AddField("player_id", playerid);
        json.AddField("hand_id", hand_index);
        JSONObject posJSON = JSONWriter.setTarget(pos);

        json.AddField("position", posJSON);
        cardJSON.AddField("playcreature", json);
        return cardJSON;
    }

    public static JSONObject AttackPlayerJSON(int attacker_id, Position attacker_pos) {
        JSONObject json = new JSONObject();
        json.AddField("attacker_id", attacker_id.ToString());
        json.AddField("target_id", -1);

        JSONObject attacker_positionJSON = JSONWriter.setTarget(attacker_pos);

        json.AddField("attacker_position", attacker_positionJSON);

        JSONObject attackCharacterJSON = new JSONObject();
        attackCharacterJSON.AddField("attack", json);
        return attackCharacterJSON;
    }

    public static JSONObject AttackCharacterJSON(int attacker_id, int target_id, Position attacker_pos, Position target_pos) {
        JSONObject json = new JSONObject();
        json.AddField("attacker_id", attacker_id.ToString());
        json.AddField("target_id", target_id.ToString());

        JSONObject attacker_positionJSON = JSONWriter.setTarget(attacker_pos);
        json.AddField("attacker_position", attacker_positionJSON);

        if (target_pos != null) {
            JSONObject target_positionJSON = JSONWriter.setTarget(target_pos);
            json.AddField("target_position", target_positionJSON);
        }

        JSONObject attackCharacterJSON = new JSONObject();
        attackCharacterJSON.AddField("attack", json);
        return attackCharacterJSON;
    }

}
