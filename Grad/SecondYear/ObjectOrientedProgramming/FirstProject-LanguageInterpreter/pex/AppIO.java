package pex;

/**
* This interface defines the methods that should be used by the core entities to write
* and read (strings and integers). This functionality depends on the type of user interaction
* that the application uses.
**/

public interface AppIO {

	/**
	* Writes a string to be presented to the user.
	*
	* @param str the string to write
	**/
	void println(String str);

	/**
	* Reads a string inputed by the user.
	*
	* @param    title   Title of the form
	* @return   the string written by the user
	**/
	public String readString(String title);

	/**
	* Reads an integer inputed by the user.
	*
	* @param	title	Title of the form
	* @return the number written by the user.
	**/
	public int readInteger(String title);
}
