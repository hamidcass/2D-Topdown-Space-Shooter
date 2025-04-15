using UnityEngine;
using System.Collections;

public class EnemyStateAlive : EnemyState {

	public override void Execute(Enemy enemy_ship){

		if (enemy_ship.IsDead()){
			enemy_ship.ChangeState(new EnemyStateDead());
		}else{		
			enemy_ship.BeAlive();
		}
		
	}
}
