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
		agent.seekTarget ();
		agent.checkRoamingLocationProximity ();
		agent.checkPlayerSeekProximity ();
	}
}

public class Seeking : State<EnemyBrain> {
	public void enter(EnemyBrain agent)
	{
		agent.setPlayerAsTarget ();
	}

	public void exit(EnemyBrain agent){}

	public void execute(EnemyBrain agent)
	{
		agent.setPlayerAsTarget ();
		agent.seekTarget (); 
		agent.checkPlayerAvoidProximity ();
		agent.fire ();
		agent.checkSeekOrRoamProximity ();
	}
}

public class FleeingPlayer : State<EnemyBrain> {

	public void enter(EnemyBrain agent)
	{
		agent.setPlayerAsTarget ();
	}

	public void exit(EnemyBrain agent){}

	public void execute(EnemyBrain agent)
	{
		agent.setPlayerAsTarget ();
		agent.fleeTarget ();
		agent.checkPlayerFleeBuffer ();
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

public class AvoidingShip : State<EnemyBrain> {

	public void enter(EnemyBrain agent)
	{
	}

	public void exit(EnemyBrain agent){}

	public void execute(EnemyBrain agent)
	{
	}
}