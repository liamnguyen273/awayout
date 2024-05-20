function connectToMetamask() {
    if (window.ethereum) {
       web3 = new Web3(window.ethereum);
        ethereum
            .request({ method: "eth_chainId" })
            .then((value) => {
                const chanId = parseInt(value, 16);
                if (chanId !== 250) {
                    connectToCorrectChainMetamask();
                } else {
                    ethereum
                        .request({ method: "eth_requestAccounts" })
                        .catch((err) => {
                            window.alert(err.message);
                        });
                    window.ethereum.on("accountsChanged", function () {
                        location.reload();
                    });
                }
            })
            .catch((err) => {
                window.alert(err.message);
            });
    } else {
        showInstallMetamask();
    }
}

function addEthereumChain () {
    var option = CHAIN_INFO[250];
     window.ethereum.request({
        method: "wallet_addEthereumChain",
        params: [
            {
                chainId: `0x${CHAIN_ID.FANTOM_OPERA.toString(16)}`,
                chainName: option.name,
                nativeCurrency: {
                    ...option.nativeCurrency,
                },
                rpcUrls: [option.rpcUrl],
                blockExplorerUrls: [option.explorer],
            },
        ],
    }).then(()=>{
        location.reload();
    });
};

function showInstallMetamask() {
    showAlert("Please install MetaMask.");
}

function showAlert(message) {
    console.log(message);
    alert(message);
}

function connectToCorrectChainMetamask() {
    window.ethereum
        .request({
            method: "wallet_switchEthereumChain",
            params: [{ chainId: `0x${CHAIN_ID.FANTOM_OPERA.toString(16)}` }],
        })
        .then(() => {
            location.reload();
        })
        .catch((err) => {
            if (err.code === 4902) {
                // Error switch chain
                addEthereumChain();
            } else {
                window.alert(err.message);
            }
        });
}

const CHAIN_ID = {
    FANTOM_TESTNET: 4002,
    FANTOM_OPERA: 250,
    KOVAN_TESTNET: 42,
    FUSE_TESTNET: 123,
    FUSE_MAINNET: 122,
    AVAX_TESTNET: 43113,
    AVAX_MAINNET: 43114,
    HUOBI_TESTNET: 256,
    HUOBI_MAINNET: 128,
};

const CHAIN_INFO = {
    [CHAIN_ID.FANTOM_OPERA]: {
        explorer: "https://ftmscan.com",
        name: "Fantom Opera",
        nativeCurrency: {
            name: "Fantom",
            symbol: "FTM",
            decimals: 18,
        },
        rpcUrl: "https://rpc.ftm.tools",
    },
    [CHAIN_ID.FANTOM_TESTNET]: {
        explorer: "https://testnet.ftmscan.com/",
        name: "Fantom Testnet",
        nativeCurrency: { name: "FTM", symbol: "FTM", decimals: 18 },
        rpcUrl: "https://rpc.testnet.fantom.network/",
    },
    [CHAIN_ID.KOVAN_TESTNET]: {
        explorer: "https://kovan.etherscan.io/",
        name: "Kovan Testnet",
        nativeCurrency: { name: "ETH", symbol: "ETH", decimals: 18 },
        rpcUrl: "https://kovan.infura.io/v3/9ad43ee2a5b84ed6968da84444c7fc91",
    },
    [CHAIN_ID.FUSE_MAINNET]: {
        explorer: "https://explorer.fuse.io/",
        name: "Fuse Mainnet",
        nativeCurrency: { name: "Fuse", symbol: "FUSE", decimals: 18 },
        rpcUrl: "https://rpc.fuse.io/",
    },
    [CHAIN_ID.FUSE_TESTNET]: {
        explorer: "https://explorer.fusespark.io/",
        name: "Fuse Testnet",
        nativeCurrency: { name: "Fuse", symbol: "FUSE", decimals: 18 },
        rpcUrl: "https://explorernode.fusespark.io/",
    },
    [CHAIN_ID.AVAX_MAINNET]: {
        explorer: "https://snowtrace.io/",
        name: "Avalanche Network",
        nativeCurrency: { name: "Avalanche", symbol: "AVAX", decimals: 18 },
        rpcUrl: "https://api.avax.network/ext/bc/C/rpc",
    },
    [CHAIN_ID.AVAX_TESTNET]: {
        explorer: "https://testnet.snowtrace.io/",
        name: "Avalanche Fuji Testnet",
        nativeCurrency: { name: "Avalanche", symbol: "AVAX", decimals: 18 },
        rpcUrl: "https://api.avax-test.network/ext/bc/C/rpc",
    },
    [CHAIN_ID.HUOBI_TESTNET]: {
        explorer: "https://scan-testnet.hecochain.com/",
        name: "Heco Testnet",
        nativeCurrency: { name: "HT", symbol: "HTT", decimals: 18 },
        rpcUrl: "https://http-testnet.hecochain.com",
    },
    [CHAIN_ID.HUOBI_MAINNET]: {
        explorer: "https://scan.hecochain.com/",
        name: "Heco Mainnet",
        nativeCurrency: { name: "HT", symbol: "HT", decimals: 18 },
        rpcUrl: "https://http-mainnet.hecochain.com/",
    },
};
