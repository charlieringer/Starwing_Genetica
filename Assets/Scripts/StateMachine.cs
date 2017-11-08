using UnityEngine;

//Basic state interface
public interface State<A>
{
	//Has 3 functions
	void enter (A agent);
	void exit (A agent);
	void execute (A agent);
}
	
public class StateMachine<A> {
	
	public A agent;
	public State<A> current;
	public StateMachine(A a) { agent = a; }

	public void init(State<A> start) {
		current = start;
		current.enter (agent);
	}
		
	public void changeState(State<A> next) {
		current.exit(agent);
		current = next;
		current.enter(agent);
	}
	public void update() {
		current.execute(agent);
	}

}