package pex.app.evaluator;

import pex.AppIO;
import pex.core.Program;

/**
* Show all program identifiers.
*/
public class ShowAllIdentifiers extends ProgramCommand{
	/**
	* @param receiver
	*/
	public ShowAllIdentifiers(Program receiver){
		super(Label.SHOW_ALL_IDENTIFIERS, receiver);
	}

	/**
	* @see pt.utl.ist.po.ui.Command#execute()
	*/
	@Override
	public final void execute(){
		AppIO appIO = entity().getInterpreter().getAppIO();
		appIO.println(entity().getAllIdentifiers());
	}
}