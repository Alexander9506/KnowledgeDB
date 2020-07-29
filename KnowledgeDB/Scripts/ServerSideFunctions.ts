
export async function uploadFormData(formData :FormData, action : string, onUploadFinished: ((success: boolean) => void)): Promise<void> {
    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    try {
        const response : Response = await fetch(action, {
            method: 'POST',
            body: formData
        });

        if (onUploadFinished != null) {
            onUploadFinished(response.ok);
        }
    } catch (error) {
        console.error('Error', error);
        onUploadFinished(false);
    }
}