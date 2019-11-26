package pex.app;

import pex.core.Interpreter;
import pex.core.Program;
import pex.AppIO;
import pex.parser.NewParser;
import pex.parser.ParserException;
import pex.app.main.MainMenu;

import pt.utl.ist.po.ui.Form;
import pt.utl.ist.po.ui.InputString;
import pt.utl.ist.po.ui.InputInteger;
import pt.utl.ist.po.ui.Display;

/**
* This is a sample client for the expression evaluator.
* It uses a text-based user interface.
*/
public class App implements java.io.Serializable, AppIO{
	private Interpreter _currentInterpreter;

	/**
	* Constructor.
	*/
	public App(){
		_currentInterpreter = new Interpreter(this);
	}

	/**
	* Gets the current Interpreter.
	*
	* @return	The current Interpreter instance
	**/
	public Interpreter getCurrentInterpreter(){
		return _currentInterpreter;
	}

	/**
	* Gets the current Interpreter.
	*
	* @param	newInterpreter	The new Interpreter
	**/
	public void setCurrentInterpreter(Interpreter newInterpreter){
		_currentInterpreter = newInterpreter;
	}

	/**
	* Writes a string to be presented to the user.
	*
	* @param 	str the string to write
	**/
	@Override
	public void println(String str){
		Display disp = new Display("");
		disp.add(str);
		disp.display();
	}

	/**
	* Reads a string inputed by the user.
	*
	* @param	title	Title of the form
	* @return 	the string written by the user
	**/
	@Override
	public String readString(String title){
		Form form = new Form("");
		InputString input = new InputString(form, title);
		form.parse();
		return input.value();
	}

	/**
	* Reads an integer inputed by the user.
	*
	* @param	title	Title of the form
	* @return the number written by the user.
	**/
	@Override
	public int readInteger(String title){
		Form form = new Form("");
		InputInteger input = new InputInteger(form, title);
		form.parse();
		return input.value();
	}

	/**
	* @param args
	*/
	public static void main(String args[]){
		App app = new App();

		String datafile = System.getProperty("import");
		if (datafile != null){
			try{
				NewParser parser = new NewParser();
				Interpreter interp = new Interpreter(app);
				Program prog = parser.parseFile(interp, datafile, "import");
				app.setCurrentInterpreter(interp);
				interp.programAdd(prog);
			}
			catch (ParserException e){ e.printStackTrace(); }
		}
		MainMenu menu = new MainMenu(app);
		menu.open();
	}
}