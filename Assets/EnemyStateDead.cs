using UnityEngine;
using System.Collections;

public class EnemyStateDead : EnemyState {

	public override void Execute(Enemy enemy_ship){
	
		enemy_ship.BeDead();
		
	}
}
