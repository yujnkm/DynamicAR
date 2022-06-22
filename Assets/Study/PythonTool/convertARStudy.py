import numpy as np
import pandas as pd
import json
import argparse

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--filepath", type=str,
                        help="path to the excel file in .xls format")
    args = parser.parse_args()
    df = pd.read_excel(args.filepath)

    df = df.replace(np.nan, '', regex=True)

    dfdict = df.to_dict(orient='records')
    titledict = dfdict[0]

    trials = []
    for j in range(1,len(dfdict)):
        trial = {}
        user = dfdict[j]
        trial["userid"] = user["userid"]
        trial["index"] = user["index"]
        sessions = []
        lastkey = "_"

        trialkey = ""
        subtrialkey = ""

        session = {}
        
        isTrial=False
        for key in titledict:
            if lastkey in key:
                trialkey = lastkey
                if(isTrial):
                    sessions.append(session)
                session = {}
                isTrial=True

            subtrialkey = titledict[key]
            session[subtrialkey] = user[key]
            lastkey = key
        sessions.append(session)
        
        trial["sessions"] = sessions
        
        trials.append(trial)

    try:
        alltrials={"trials":trials}
        jsonString = json.dumps(alltrials)
        jsonFile = open(args.filepath.replace(".xls",".json"), "w")
        jsonFile.write(jsonString)
        jsonFile.close()
        print("Conversion completed.")
    except:
        print("Conversion Error.")
    

if __name__ == "__main__":
    main()