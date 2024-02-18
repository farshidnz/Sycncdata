 import * as cdk from 'aws-cdk-lib';
 import { Template } from 'aws-cdk-lib/assertions';
 import { DeployStack } from "../lib/deploy-stack";
 import { StartsWithMatch } from "./helper";

// example test. To run these tests, uncomment this file along with the
// example resource in lib/deploy-stack.ts
function setupEnvironment() {
  process.env.ENVIRONMENT_NAME = 'wearemc';
  process.env.AWS_ACCOUNT_ID = '752830773963';
  process.env.AWS_REGION = 'ap-southeast-2';
  process.env.PROJECT_NAME = 'Accounts';
  process.env.SERVICE_NAME = 'SyncData';
  process.env.CODE_BUCKET = 'cashrewards-lambda-packages';
  process.env.CODE_PATH = `${process.env.ENVIRONMENT_NAME}/${process.env.PROJECT_NAME}/AccountsSyncData.zip`;
  process.env.ScalingMinCapacity = '0';
  process.env.ScalingMaxCapacity = '10';
  process.env.timeOut = '15';
  process.env.memoryLimitMiB = '1024';
  process.env.ConcurrentExecutions = '5';
  process.env.ScalingUtilizationTarget = '0.6';
  process.env.MaximumMessageCount = '120';
  process.env.MemberTopicArn = "arn:aws:sns:ap-southeast-2:752830773963:Test"
}

describe("Should create AccountsSyncData cdk stack", () => {
  setupEnvironment();
  const environmentName = process.env.ENVIRONMENT_NAME;
  const account = process.env.AWS_ACCOUNT_ID;
  const region = process.env.AWS_REGION;
  const projectName = process.env.PROJECT_NAME || 'Accounts';
  const serviceName = process.env.SERVICE_NAME;

  const app = new cdk.App();

  const stack = new DeployStack(app, projectName, {
    env: {
        account,
        region,
    },
  });

  const template = Template.fromStack(stack);

  test("Should create lambda function", () => {

    template.hasResourceProperties("AWS::Lambda::Function", {
      Code: {
        S3Bucket:  process.env.CODE_BUCKET,
        S3Key: process.env.CODE_PATH
      },
      Role: {
        "Fn::GetAtt": [
          new StartsWithMatch('',
            `${environmentName}${projectName}FunctionServiceRole`
          ),
          "Arn"
        ]
      },
      Environment: {
        Variables: {
          BASE_PATH: serviceName,
          ENVIRONMENT_NAME: environmentName,
        }
      },
      "Handler": "Function.FunctionHandler",
      MemorySize: +`${process.env.memoryLimitMiB}`,
      Runtime: "dotnet6",
      Tags: [
        {
          "Key": "Application",
          "Value": `${projectName}${serviceName}`
        },
        {
          "Key": "Team",
          "Value": projectName
        }
      ],
      Timeout: +`${process.env.timeOut}`
    });
  });
  

  test("Should Subscribe to Member Topic", () => {
    template.hasResourceProperties("AWS::SNS::Subscription", {
      Protocol: "sqs",
      Endpoint: {
        "Fn::GetAtt": [
          new StartsWithMatch('',
            `${process.env.ENVIRONMENT_NAME}MemberTopicSubscription`
          ),
          "Arn"
       ]
      },
      TopicArn:
         "arn:aws:sns:ap-southeast-2:752830773963:Test",
      FilterPolicy: {
        EventType: ["MemberCreated", "MemberDetailUpdated"],
      },
      RawMessageDelivery: true,
      Region: "ap-southeast-2"
    });
  });

});