package pex.app.main;

import java.io.IOException;
import java.lang.NullPointerException;
import java.io.FileOutputStream;
import java.io.ObjectOutputStream;

import pex.app.App;
import pex.core.Interpreter;
import pex.app.main.Message;
import pt.utl.ist.po.ui.Command;
import pt.utl.ist.po.ui.Form;
import pt.utl.ist.po.ui.InputString;
import pt.utl.ist.po.ui.InvalidOperation;

/**
* Save to file under current name (if unnamed, query for name).
*/
public class Save extends Command<App>{
	/**
	* @param receiver
	*/
	public Save(App receiver){
		super(Label.SAVE, receiver);
	}

	/**
	* @see pt.utl.ist.po.ui.Command#execute()
	*/
	@Override
	public final void execute() throws InvalidOperation{
		try{
			Interpreter interp = entity().getCurrentInterpreter();
			String filepath = interp.getFileName();
			if (filepath.equals("")){
				filepath = entity().readString(Message.newSaveAs());
				interp.setFileName(filepath);
			}
			interp.interpreterSave();
		}
		catch(IOException | NullPointerException e){
			e.printStackTrace();
		}
	}
}