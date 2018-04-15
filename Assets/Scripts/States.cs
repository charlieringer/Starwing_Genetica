using UnityEngine;
using System.Collections;


public class Roaming: State<EnemyBrain> {
	public void enter(EnemyBrain agent)
	{
		agent.pickRandomRoamingTarget();

	}

	public void exit(EnemyBrain agent){}

	public void execute(EnemyBrain agent)
	{
		agent.roamToTarget ();
		agent.checkRoamingLocationProximity ();
		agent.checkPlayerSeekProximity ();
	}
}

public class Seeking : State<EnemyBrain> {
	public void enter(EnemyBrain agent)
	{
		agent.setPlayerAsTarget ();
		agent.startSeekTimer ();
	}

	public void exit(EnemyBrain agent){}

	public void execute(EnemyBrain agent)
	{
		agent.setPlayerAsTarget ();
		agent.seekTarget (); 
		bool changedState = agent.checkPlayerAvoidProximity ();
		if (changedState) return;
		agent.fire ();
		agent.checkSeekOrRoamProximity ();
	}
}

public class FleeingPlayer : State<EnemyBrain> {

	public void enter(EnemyBrain agent)
	{
		//agent.setPlayerAsTarget ();
		agent.pickRandomRoamingTarget();
		Debug.Log("In Flee Mode");
	}

	public void exit(EnemyBrain agent){
		Debug.Log("Out Flee Mode");
	}

	public void execute(EnemyBrain agent)
	{
		agent.roamToTarget ();
		agent.checkFleeLocationProximity ();
	}
}

public class AvoidingBullet : State<EnemyBrain> {

	public void enter(EnemyBrain agent)
	{
	}

	public void exit(EnemyBrain agent){}

	public void execute(EnemyBrain agent)
	{
	}
}