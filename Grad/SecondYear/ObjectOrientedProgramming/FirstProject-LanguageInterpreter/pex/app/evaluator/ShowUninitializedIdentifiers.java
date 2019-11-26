package pex.app.evaluator;

import pex.AppIO;
import pex.core.Program;

/**
* Show uninitialized identifiers.
*/
public class ShowUninitializedIdentifiers extends ProgramCommand{
	/**
	* @param receiver
	*/
	public ShowUninitializedIdentifiers(Program receiver){
		super(Label.SHOW_UNINITIALIZED_IDENTIFIERS, receiver);
	}

	/**
	* @see pt.utl.ist.po.ui.Command#execute()
	*/
	@Override
	public final void execute(){
		AppIO appIO = entity().getInterpreter().getAppIO();
		appIO.println(entity().getUninitializedIdentifiers());
	}
}