using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GPS_Manager : MonoBehaviour
{
    //텍스트 UI 변수
    public Text latitude_text;
    public Text longtitude_text;
    public float maxWaitTime = 10.0f;

    //위도 경도 변수
    public float latitude = 0;
    public float longtitude = 0;

    float waitTime = 0;

    void Start()
    {
        StartCoroutine(GPS_On());
    }

    //GPS 처리 함수
    public IEnumerator GPS_On()
    {
        //만일 GPS 사용허가를 받지 못했다면 권한 허가 팝업을 띄운다
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            while(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        //만일, GPS 장치가 켜져 있지 않으면 위치 정보를 수신할 수 없다고 표시한다
        if(!Input.location.isEnabledByUser)
        {
            latitude_text.text = "GPS Off";
            longtitude_text.text = "GPS Off";
            yield break;
        }

        //위치 데이터를 요청한다 -> 수신 대기
        Input.location.Start();

        //GPS 수신 상태가 초기 상태에서 일정 시간 동안 대기한다
        while(Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWaitTime)
        {
            yield return new WaitForSeconds(1.0f);
            waitTime++;
        }

        //수신 실패 시 수신이 실패됐다는 것을 출력
        if(Input.location.status == LocationServiceStatus.Failed)
        {
            latitude_text.text = "위치 정보 수신 실패";
            longtitude_text.text = "위치 정보 수신 실패";
        }

        //응답 대기 시간을 넘어가도록 수신이 없었다면 시간 초과됐음을 출력한다
        if(waitTime >= maxWaitTime)
        {
            latitude_text.text = "응답 대기 시간 초과";
            longtitude_text.text = "응답 대기 시간 초과";
        }

        //수신된 GPS 데이터를 화면에 출력한다
        LocationInfo li = Input.location.lastData;
        latitude = li.latitude;
        longtitude = li.longitude;
        latitude_text.text = "위도 : " + latitude.ToString();
        longtitude_text.text = "경도 : " + longtitude.ToString();
    }
}
