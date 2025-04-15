using UnityEngine;
using System.Collections;

public class PlayerStateDead : PlayerState {

	public override void Execute(Player player_ship){
	
		player_ship.BeDead();
		
	}
}
