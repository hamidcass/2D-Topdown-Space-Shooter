using UnityEngine;
using System.Collections;

public class PlayerStateAlive : PlayerState {

	public override void Execute(Player player_ship){

		if (player_ship.IsDead()){
			player_ship.ChangeState(new PlayerStateDead());
		}else{		
			player_ship.BeAlive();
		}
		
	}
}
