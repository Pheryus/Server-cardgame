using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JSONReader {

    
    public static Position getPositionInstance(JSONObject positionJSON, bool hasSide = true) {
        int line = (int)positionJSON.GetField("line").n;
        int column = (int)positionJSON.GetField("column").n;
        if (hasSide) { 
            int side = (int)positionJSON.GetField("side").n;
            return new Position(line, column, side);
        }
        return new Position(line, column);

    }

}
