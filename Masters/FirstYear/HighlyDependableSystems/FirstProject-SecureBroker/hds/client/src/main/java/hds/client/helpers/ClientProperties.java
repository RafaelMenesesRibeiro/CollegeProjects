package hds.client.helpers;

import java.io.IOException;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.spec.InvalidKeySpecException;
import java.util.ArrayList;
import java.util.List;

import static hds.security.ResourceManager.getPrivateKeyFromResource;

public class ClientProperties {
    public static final int HDS_NOTARY_CLIENTS_FIRST_PORT = 8000;
    private static final int HDS_NOTARY_REPLICAS_FIRST_PORT = 9000;
    private static final int HDS_NOTARY_REPLICAS_FIRST_CC_PORT = 10000;

    public static final String HDS_BASE_HOST = "http://localhost:";

    private static PrivateKey myPrivateKey = null;
    private static List<String> regularReplicaIdList = new ArrayList<>();
    private static List<String> citizenReplicaIdList = new ArrayList<>();
    private static List<String> replicasList = new ArrayList<>();
    private static Integer numberOfReplicas = 0;
    private static Integer maxFailures = 0;
    private static Integer majorityThreshold = 0;
    private static String myClientPort = "8000";

    private ClientProperties() {}

    public static PrivateKey getMyPrivateKey() {
        return myPrivateKey;
    }

    private static void setMyPrivateKey(PrivateKey myPrivateKey) {
        ClientProperties.myPrivateKey = myPrivateKey;
    }

    private static List<String> getRegularReplicaIdList() {
        return regularReplicaIdList;
    }

    private static void setRegularReplicaIdList(List<String> regularReplicaIdList) {
        ClientProperties.regularReplicaIdList = regularReplicaIdList;
    }

    private static List<String> getCitizenReplicaIdList() {
        return citizenReplicaIdList;
    }

    private static void setCitizenReplicaIdList(List<String> citizenReplicaIdList) {
        ClientProperties.citizenReplicaIdList = citizenReplicaIdList;
    }

    public static List<String> getReplicasList() {
        return replicasList;
    }

    private static void setReplicasList() {
        ClientProperties.replicasList.addAll(regularReplicaIdList);
        ClientProperties.replicasList.addAll(citizenReplicaIdList);
        setNumberOfReplicas();
    }

    public static Integer getNumberOfReplicas() {
        return numberOfReplicas;
    }

    private static void setNumberOfReplicas() {
        ClientProperties.numberOfReplicas = replicasList.size();
    }

    public static Integer getMaxFailures() {
        return maxFailures;
    }

    private static void setMaxFailures(Integer maxFailures) {
        ClientProperties.maxFailures = maxFailures;
    }

    public static String getMyClientPort() {
        return myClientPort;
    }

    private static void setMyClientPort(String myClientPort) {
        ClientProperties.myClientPort = myClientPort;
    }

    private static void setMajorityThreshold() {
        ClientProperties.majorityThreshold = (numberOfReplicas + maxFailures) / 2;
    }

    public static int getMajorityThreshold() {
        return majorityThreshold;
    }

    /**************************************
     *  CLIENT APPLICATION INITIALIZERS
     *************************************/

    private static void initRegularReplicasIdList(int number) {
        int maxPort = HDS_NOTARY_REPLICAS_FIRST_PORT + number - 1;
        setRegularReplicaIdList(newReplicasList(HDS_NOTARY_REPLICAS_FIRST_PORT, maxPort));
    }

    private static void initCitizenReplicaIdList(int number) {
        int maxPort = HDS_NOTARY_REPLICAS_FIRST_CC_PORT + number - 1;
        setCitizenReplicaIdList(newReplicasList(HDS_NOTARY_REPLICAS_FIRST_CC_PORT, maxPort));
    }

    private static ArrayList<String> newReplicasList(int start, int end) {
        ArrayList<String> replicas = new ArrayList<>();
        for (int replicaPort = start; replicaPort <= end; replicaPort++) {
            replicas.add("" + replicaPort);
        }
        return replicas;
    }

    /**************************************
     *  HELPERS
     *************************************/

    public static void print(String msg) {
        System.out.println("[o] " + msg);
    }

    public static void printError(String msg) {
        System.out.println("    [x] " + msg);
    }

    public static void init(String portId, int maxFailures, int regularReplicasNumber, int ccReplicasNumber) {
        ClientProperties.setMyClientPort(portId);
        try {
            ClientProperties.setMyPrivateKey(getPrivateKeyFromResource(portId));
        } catch (NoSuchAlgorithmException | IOException | InvalidKeySpecException exc) {
            System.exit(1);
        }
        ClientProperties.setMaxFailures(maxFailures);
        ClientProperties.initRegularReplicasIdList(regularReplicasNumber);
        ClientProperties.initCitizenReplicaIdList(ccReplicasNumber);
        ClientProperties.setReplicasList();
        ClientProperties.setMajorityThreshold();
    }
}
