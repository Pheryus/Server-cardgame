using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JSONReader {

    
    public static Position getPositionInstance(JSONObject positionJSON, bool hasSide = true) {

        Debug.Log("json: " + positionJSON);

        int line = (int)positionJSON.GetField("line").n;
        int column = (int)positionJSON.GetField("column").n;
        if (hasSide) { 
            int side = (int)positionJSON.GetField("side").n;
            return new Position(column, line, side);
        }
        return new Position(column, line);

    }

}
