//function loginWithGoogle() {
//    const provider = new firebase.auth.GoogleAuthProvider();
//    firebase.auth().signInWithPopup(provider)
//        .then((result) => {
//            const idToken = result.credential.idToken;
//            window.location.href = `/Auth/GoogleLogin?idToken=${idToken}`;
//        })
//        .catch((error) => {
//            console.error('Google login error:', error);
//            alert('Google login failed: ' + error.message);
//        });
//}