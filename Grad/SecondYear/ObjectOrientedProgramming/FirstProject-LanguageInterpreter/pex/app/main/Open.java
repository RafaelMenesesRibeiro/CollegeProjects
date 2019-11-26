package pex.app.main;

import java.io.IOException;
import java.io.FileNotFoundException;
import java.lang.ClassNotFoundException;

import pex.app.App;
import pex.core.Interpreter;
import pex.app.main.Message;
import pt.utl.ist.po.ui.Command;
import pt.utl.ist.po.ui.InvalidOperation;

/**
* Open existing interpreter.
*/
public class Open extends Command<App>{
	/**
	* @param receiver
	*/
	public Open(App receiver){
		super(Label.OPEN, receiver);
	}

	/**
	* @see pt.tecnico.po.ui.Command#execute()
	*/
	@Override
	public final void execute() throws InvalidOperation{
		String filepath = "";
		try{
			filepath = entity().readString(Message.openFile());
			Interpreter newInterp = Interpreter.interpreterOpen(filepath);
			entity().setCurrentInterpreter(newInterp);
		}
		catch(FileNotFoundException e1){
			entity().println(Message.fileNotFound(filepath));
		}
		catch(ClassNotFoundException | IOException e2){
			e2.printStackTrace();
		}
	}
}
