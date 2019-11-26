package pex.app.main;

import pex.app.App;
import pex.core.Program;
import pex.app.evaluator.EvaluatorMenu;
import pt.utl.ist.po.ui.Command;
import pt.utl.ist.po.ui.InputString;
import pt.utl.ist.po.ui.Menu;
import pt.utl.ist.po.ui.Display;
import pt.utl.ist.po.ui.Form;


/**
* Open menu for managing programs.
*/
public class EditProgram extends Command<App>{

	/**
	* @param receiver
	*/
	public EditProgram(App receiver){
		super(Label.MANAGE_PROGRAM, receiver);
	}

	/**
	* @see pt.utl.ist.po.ui.Command#execute()
	*/
	@Override
	public final void execute(){
		String progName = entity().readString(Message.requestProgramId());
		Program prog = entity().getCurrentInterpreter().programFind(progName);
		if (prog == null){
			entity().println(Message.noSuchProgram(progName));
			return;
		}
		Menu evaluatorMenu = new EvaluatorMenu(prog);
		evaluatorMenu.open();
	}
}