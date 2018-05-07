using UnityEngine;
using UnityEngine.UI;
using Loom.Unity3d;
using Newtonsoft.Json;
using Google.Protobuf;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class etherboySample : MonoBehaviour
{
    private Address contractAddr;
	public Identity identity;
    private Address callerAddr;
    private DAppChainClient chainClient;

    // Use this for initialization
    void Start()
    {
        // By default the editor won't respond to network IO or anything if it doesn't have input focus,
        // which is super annoying when input focus is given to the web browser for the Auth0 sign-in.
		DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IAuthClient CreateAuthClient()
    {
#if !UNITY_WEBGL
        try
        {
            CertValidationBypass.Enable();
            return AuthClientFactory.Configure()
                .WithLogger(Debug.unityLogger)
                .WithClientId("25pDQvX4O5j7wgwT052Sh3UzXVR9X6Ud") // unity3d sdk
                .WithDomain("loomx.auth0.com")
                .WithScheme("io.loomx.unity3d")
                .WithAudience("https://keystore.loomx.io/")
                .WithScope("openid profile email picture")
                .WithRedirectUrl("http://127.0.0.1:9999/auth/auth0/")
                .Create();
        }
        finally
        {
            CertValidationBypass.Disable();
        }
#else
        return AuthClientFactory.Configure()
            .WithLogger(Debug.unityLogger)
            .WithHostPageHandlers(new Loom.Unity3d.WebGL.HostPageHandlers
            {
                SignIn = "authenticateFromGame",
                GetUserInfo = "getUserInfo",
                SignOut = "clearUserInfo"
            })
            .Create();
#endif
    }

#if !UNITY_WEBGL // In WebGL all interactions with the key store should be done in the host page.
    private async Task<IKeyStore> CreateKeyStore(string accessToken)
    {
        return await KeyStoreFactory.CreateVaultStore(new VaultStoreConfig
        {
            Url = "https://stage-vault.delegatecall.com/v1/",
            VaultPrefix = "unity3d-sdk",
            AccessToken = accessToken
        });
    }
#endif

	public void createIdentity () {
		//System.Text.Encoding.UTF8.GetString(this.identity.PrivateKey)
	}

	public async void SignIn()
    {
#if !UNITY_WEBGL
        try
        {
            CertValidationBypass.Enable();
            var authClient = this.CreateAuthClient();
            var accessToken = await authClient.GetAccessTokenAsync();
            var keyStore = await this.CreateKeyStore(accessToken);
            this.identity = await authClient.GetIdentityAsync(accessToken, keyStore);
        }
        finally
        {
            CertValidationBypass.Disable();
        }
#else
        var authClient = this.CreateAuthClient();
        this.identity = await authClient.GetIdentityAsync("", null);
#endif
		PlayerPrefs.SetString("identityString", System.Text.Encoding.UTF8.GetString(this.identity.PrivateKey));
        // This DAppChain client will connect to the example REST server in the Loom Go SDK. 
        //this.chainClient = new DAppChainClient("http://localhost:46657", "http://localhost:47000")
        //this.chainClient = new DAppChainClient("http://etherboy-stage.loomapps.io", 46657, 9999)
        this.chainClient = new DAppChainClient("http://etherboy-write-stage.loomapps.io", "http://etherboy-read-stage.loomapps.io")
        {
            Logger = Debug.unityLogger
        };
        this.chainClient.TxMiddleware = new TxMiddleware(new ITxMiddlewareHandler[]{
            new NonceTxMiddleware{
                PublicKey = this.identity.PublicKey,
                Client = this.chainClient
            },
            new SignedTxMiddleware(this.identity.PrivateKey)
        });

        // There is only one contract address at the moment...
        this.contractAddr = new Address
        {
            ChainId = "default",
            Local = ByteString.CopyFrom(CryptoUtils.HexStringToBytes("0x005B17864f3adbF53b1384F2E6f2120c6652F779"))
        };
        this.callerAddr = this.identity.ToAddress("default");
		CreateAccount ();
    }

    public async void SignOut()
    {
        var authClient = this.CreateAuthClient();
        await authClient.ClearIdentityAsync();
    }

    public async void ResetPrivateKey()
    {
#if !UNITY_WEBGL
        try
        {
            CertValidationBypass.Enable();
            var authClient = this.CreateAuthClient();
            var accessToken = await authClient.GetAccessTokenAsync();
            var keyStore = await this.CreateKeyStore(accessToken);
            this.identity = await authClient.CreateIdentityAsync(accessToken, keyStore);
        }
        finally
        {
            CertValidationBypass.Disable();
        }
#else
        // TODO
        throw new NotImplementedException();
#endif
    }

    // The backend doesn't care what the account info structure looks like,
    // it just needs to be serializable to JSON.
    // NOTE: Don't store any private info like email.
    private class SampleAccountInfo
    {
        public DateTime CreatedOn { get; set; }
    }

	public async void CreateAccount()
    {
        if (this.identity == null)
        {
            throw new System.Exception("Not signed in!");
        }
        // Create new player account
        var accountInfo = JsonConvert.SerializeObject(new SampleAccountInfo
        {
            CreatedOn = DateTime.Now
        });
        var createAcctTx = new EtherboyCreateAccountTx
        {
            Version = 0,
			Owner = this.identity.Username,
            Data = ByteString.CopyFromUtf8(accountInfo)
        };

		try {
        	var result = await this.chainClient.CallAsync(this.callerAddr, this.contractAddr, "etherboycore.CreateAccount", createAcctTx);
		} catch (System.Exception ex) {
			print (ex.ToString());
		}
    }

    // The backend doesn't care what the state structure looks like,
    // it just needs to be serializable to JSON.

	public async void SaveState(SampleState saveData)
    {
        if (this.identity == null)
        {
            throw new System.Exception("Not signed in!");
        }

        // Save state to the backend
		var state = JsonConvert.SerializeObject(saveData);
        var saveStateTx = new EtherboyStateTx
        {
            Version = 0,
			Owner = this.identity.Username,
            Data = ByteString.CopyFromUtf8(state)
        };
        
        var result = await this.chainClient.CallAsync(this.callerAddr, this.contractAddr, "etherboycore.SaveState", saveStateTx); 
    }

	public async void QuerySaveData()
    {
        // NOTE: Query results can be of any type that can be deserialized via Newtonsoft.Json.
        var result = await this.chainClient.QueryAsync<StateQueryResult>(
			this.contractAddr, "etherboycore.GetState", new StateQueryParams{ Owner = this.identity.Username }
        );
        // TODO: This is clunkier than before, try to remove this extra deserialization step... handle it in QueryAsync maybe.
		globalScript.loadGame(JsonConvert.DeserializeObject<SampleState>(result.State.ToStringUtf8()));
    }
}
