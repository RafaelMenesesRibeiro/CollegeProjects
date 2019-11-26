package pex.app.main;

import pex.app.App;
import pex.core.Program;
import pex.core.Interpreter;
import pt.utl.ist.po.ui.Command;
import pt.utl.ist.po.ui.Form;
import pt.utl.ist.po.ui.InputString;

/**
* Create new program.
*/
public class NewProgram extends Command<App>{
	/**
	* @param receiver
	*/
	public NewProgram(App receiver){
		super(Label.NEW_PROGRAM, receiver);
	}

	/**
	* @see pt.utl.ist.po.ui.Command#execute()
	*/
	@Override
	public final void execute(){
		String name = entity().readString(Message.requestProgramId());
		Program aux = new Program(entity().getCurrentInterpreter(), name);
		entity().getCurrentInterpreter().programAdd(aux);
	}
}