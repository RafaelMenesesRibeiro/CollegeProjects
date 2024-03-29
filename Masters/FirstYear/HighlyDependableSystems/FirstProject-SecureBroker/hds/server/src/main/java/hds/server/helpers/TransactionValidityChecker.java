package hds.server.helpers;

import hds.security.CryptoUtils;
import hds.security.exceptions.SignatureException;
import hds.security.msgtypes.ApproveSaleRequestMessage;
import hds.security.msgtypes.SaleRequestMessage;
import hds.server.exception.DBClosedConnectionException;
import hds.server.exception.DBConnectionRefusedException;
import hds.server.exception.DBNoResultsException;
import hds.server.exception.IncorrectSignatureException;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.security.NoSuchAlgorithmException;
import java.security.PublicKey;
import java.security.spec.InvalidKeySpecException;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import static hds.security.ResourceManager.getPublicKeyFromResource;
import static hds.server.helpers.DatabaseInterface.queryDB;

/**
 * Verifies if the transaction is valid.
 *
 * @author 		Diogo Vilela
 * @author 		Francisco Barros
 * @author 		Rafael Ribeiro
 */
@SuppressWarnings("Duplicates")
public class TransactionValidityChecker {
	private TransactionValidityChecker() {
		// This is here so the class can't be instantiated. //
	}

	/**
	 * Verifies if the transaction is valid.
	 * Confirms authenticity and integrity of the request and wrapped request.
	 * Confirms the SellerID owns the GoodID.
	 * Confirms the GoodID is on sale.
	 *
	 * @param   conn        	Database connection
	 * @param 	transactionData	ApproveSaleRequestMessage with GoodID, BuyerID, SellerID and all the signatures
	 * @return 	boolean			Represents if the transaction is valid.
	 * @throws  SQLException                    The DB threw an SQLException
	 * @throws 	DBClosedConnectionException		Can't access the DB
	 * @throws 	DBConnectionRefusedException	Can't access the DB
	 * @throws 	DBNoResultsException			The DB did not return any results
	 * @throws 	SignatureException				Couldn't verify the payload's signature
	 * @throws	IncorrectSignatureException		The payload's signature does not match its contents
	 */
	public static boolean isValidTransaction(Connection conn, ApproveSaleRequestMessage transactionData)
			throws JSONException, SQLException, DBClosedConnectionException, DBConnectionRefusedException, DBNoResultsException,
					SignatureException, IncorrectSignatureException {

		String buyerID = transactionData.getBuyerID();
		String sellerID = transactionData.getSellerID();
		String goodID = transactionData.getGoodID();

		SaleRequestMessage saleRequestMessage = new SaleRequestMessage(
				transactionData.getTimestamp(),
				transactionData.getRequestID(),
				transactionData.getOperation(),
				transactionData.getFrom(),
				transactionData.getTo(),
				"",
				transactionData.getGoodID(),
				transactionData.getBuyerID(),
				transactionData.getSellerID(),
				transactionData.getWts(),
				transactionData.getOnSale(),
				transactionData.getWriteOnGoodsSignature(),
				transactionData.getWriteOnOwnershipsSignature()
		);

		if (!isClientWilling(buyerID, transactionData.getSignature(), saleRequestMessage)) {
			throw new IncorrectSignatureException("The Buyer's signature is not valid.");
		}

		String currentOwner = getCurrentOwner(conn, goodID);
		return (currentOwner.equals(sellerID) && getIsOnSale(conn, goodID));
	}

	/**
	 * Gets the current owner of the GoodID.
	 *
	 * @param   conn        Database connection
	 * @param 	goodID		GoodID
	 * @return 	String		ID of the GoodID's owner
	 * @throws 	JSONException					Can't create / parse JSONObject
	 * @throws  SQLException                    The DB threw an SQLException
	 * @throws 	DBClosedConnectionException		Can't access the DB
	 * @throws 	DBConnectionRefusedException	Can't access the DB
	 * @throws 	DBNoResultsException			The DB did not return any results
	 */
	public static String getCurrentOwner(Connection conn, String goodID)
			throws JSONException, SQLException, DBClosedConnectionException, DBConnectionRefusedException, DBNoResultsException {

		String query = "select userID from ownership where goodId = ?";
		List<String> args = new ArrayList<>();
		args.add(goodID);

		String columnName = "userID";
		List<String> returnColumns = new ArrayList<>();
		returnColumns.add(columnName);

		List<JSONObject> results = queryDB(conn, query, returnColumns, args);
		return results.get(0).getString(columnName);
	}

	/**
	 * Checks if the GoodID is on sale in the database.
	 *
	 * @param   conn        Database connection
	 * @param 	goodID		GoodID
	 * @return 	Boolean		Represents if the GoodID is on sale
	 * @throws 	JSONException					Can't create / parse JSONObject
	 * @throws  SQLException                    The DB threw an SQLException
	 * @throws 	DBClosedConnectionException		Can't access the DB
	 * @throws 	DBConnectionRefusedException	Can't access the DB
	 * @throws 	DBNoResultsException			The DB did not return any results
	 */
	public static Boolean getIsOnSale(Connection conn, String goodID)
			throws JSONException, SQLException, DBClosedConnectionException, DBConnectionRefusedException, DBNoResultsException {

		String query = "select onSale from goods where goodID = ?";
		List<String> args = new ArrayList<>();
		args.add(goodID);

		String columnName = "onSale";
		List<String> returnColumns = new ArrayList<>();
		returnColumns.add(columnName);

		List<JSONObject> results = queryDB(conn, query, returnColumns, args);
		return results.get(0).getString(columnName).equals("t");
	}

	/**
	 * Returns the information on the database's Ownership table for a certain GoodID.
	 *
	 * @param   conn        Database connection
	 * @param 	goodID		GoodID
	 * @return 	JSONObject	Contains all the columns' values for the GoodID
	 * @throws 	JSONException					Can't create / parse JSONObject
	 * @throws  SQLException                    The DB threw an SQLException
	 * @throws 	DBClosedConnectionException		Can't access the DB
	 * @throws 	DBConnectionRefusedException	Can't access the DB
	 * @throws 	DBNoResultsException			The DB did not return any results
	 */
	public static JSONObject getOnOwnershipInfo(Connection conn, String goodID)
			throws JSONException, SQLException, DBClosedConnectionException, DBConnectionRefusedException, DBNoResultsException {

		String query = "SELECT * FROM ownership WHERE goodId = ?";
		List<String> args = new ArrayList<>();
		args.add(goodID);

		List<String> returnColumns = new ArrayList<>();
		returnColumns.add("goodID");
		returnColumns.add("userID");
		returnColumns.add("ts");
		returnColumns.add("sig");

		List<JSONObject> results = queryDB(conn, query, returnColumns, args);
		return results.get(0);
	}

	/**
	 * Returns the information on the database's Goods table for a certain GoodID.
	 *
	 * @param   conn        Database connection
	 * @param 	goodID		GoodID
	 * @return 	JSONObject	Contains all the columns' values for the GoodID
	 * @throws 	JSONException					Can't create / parse JSONObject
	 * @throws  SQLException                    The DB threw an SQLException
	 * @throws 	DBClosedConnectionException		Can't access the DB
	 * @throws 	DBConnectionRefusedException	Can't access the DB
	 * @throws 	DBNoResultsException			The DB did not return any results
	 */
	public static JSONObject getOnGoodsInfo(Connection conn, String goodID)
			throws JSONException, SQLException, DBClosedConnectionException, DBConnectionRefusedException, DBNoResultsException {

		String query = "SELECT * FROM goods WHERE goodId = ?";
		List<String> args = new ArrayList<>();
		args.add(goodID);

		List<String> returnColumns = new ArrayList<>();
		returnColumns.add("goodID");
		returnColumns.add("onSale");
		returnColumns.add("wid");
		returnColumns.add("ts");
		returnColumns.add("sig");

		List<JSONObject> results = queryDB(conn, query, returnColumns, args);
		return results.get(0);
	}

	/**
	 * Returns the write timestamp on the database's Goods table for a certain GoodID entry.
	 *
	 * @param   connection	Database connection
	 * @param 	goodID		GoodID
	 * @return 	long		Write timestamp
	 * @throws 	JSONException					Can't create / parse JSONObject
	 * @throws  SQLException                    The DB threw an SQLException
	 * @throws 	DBClosedConnectionException		Can't access the DB
	 * @throws 	DBConnectionRefusedException	Can't access the DB
	 * @throws 	DBNoResultsException			The DB did not return any results
	 */
	public static long getOnGoodsTimestamp(Connection connection, String goodID)
			throws SQLException, DBClosedConnectionException, DBConnectionRefusedException, DBNoResultsException, JSONException {
		String query = "SELECT ts FROM goods WHERE goodId = ?";
		List<String> args = new ArrayList<>();
		args.add(goodID);

		List<String> returnColumns = new ArrayList<>();
		String returnColumn = "ts";
		returnColumns.add(returnColumn);
		try {
			List<JSONObject> results = queryDB(connection, query, returnColumns, args);
			JSONObject json = results.get(0);
			String timestamp = json.getString(returnColumn);
			return Long.parseLong(timestamp);
		}
		// DBClosedConnectionException | DBConnectionRefusedException | DBNoResultsException
		// are ignored to be caught up the chain.
		catch (IndexOutOfBoundsException | NullPointerException ex) {
			throw new DBNoResultsException("The query \"" + query + "\" returned no results.");
		}
	}

	/**
	 * Returns the write timestamp on the database's Ownership table for a certain GoodID entry.
	 *
	 * @param   connection	Database connection
	 * @param 	goodID		GoodID
	 * @return 	long		Write timestamp
	 * @throws 	JSONException					Can't create / parse JSONObject
	 * @throws  SQLException                    The DB threw an SQLException
	 * @throws 	DBClosedConnectionException		Can't access the DB
	 * @throws 	DBConnectionRefusedException	Can't access the DB
	 * @throws 	DBNoResultsException			The DB did not return any results
	 */
	public static long getOnOwnershipTimestamp(Connection connection, String goodID)
			throws SQLException, DBClosedConnectionException, DBConnectionRefusedException, DBNoResultsException, JSONException {
		String query = "SELECT ts FROM ownership WHERE goodId = ?";
		List<String> args = new ArrayList<>();
		args.add(goodID);

		List<String> returnColumns = new ArrayList<>();
		String returnColumn = "ts";
		returnColumns.add(returnColumn);
		try {
			List<JSONObject> results = queryDB(connection, query, returnColumns, args);
			JSONObject json = results.get(0);
			String timestamp = json.getString(returnColumn);
			return Long.parseLong(timestamp);
		}
		// DBClosedConnectionException | DBConnectionRefusedException | DBNoResultsException
		// are ignored to be caught up the chain.
		catch (IndexOutOfBoundsException | NullPointerException ex) {
			throw new DBNoResultsException("The query \"" + query + "\" returned no results.");
		}
	}

	/**
	 * Verifies the signature of the payload.
	 *
	 * @param 	clientID		The ClientID that sent the payload
	 * @param 	buyerSignature	The Signature's String of the payload
	 * @param 	payload			The payload that was signed
	 * @throws  SignatureException              Couldn't sign the payload
	 */
	public static boolean isClientWilling(String clientID, String buyerSignature, Object payload)
			throws SignatureException {
		try {
			PublicKey buyerPublicKey = getPublicKeyFromResource(clientID);
			return CryptoUtils.authenticateSignatureWithPubKey(buyerPublicKey, buyerSignature, payload.toString());
		}
		catch (NullPointerException | IOException | InvalidKeySpecException | NoSuchAlgorithmException | java.security.SignatureException e) {
			throw new SignatureException(e.getMessage());
		}
	}
}
