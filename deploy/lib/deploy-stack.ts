import { Runtime } from 'aws-cdk-lib/aws-lambda';
import { Stack, StackProps } from 'aws-cdk-lib';
import { CRLambdaAlbConstruct, DomainTopicSubscriptionConstruct, DomainEventFilter, ServiceVisibility, getEnv, getResourceName, applyMetaTags } from '@cashrewards/cdk-lib';
import { Construct } from 'constructs';
import { PolicyStatement } from "aws-cdk-lib/aws-iam";

export class DeployStack extends Stack {
  public lambdaConstruct: CRLambdaAlbConstruct;
  public accountsTopicSub: DomainTopicSubscriptionConstruct;

  constructor(scope: Construct, id: string, props?: StackProps) {
    super(scope, id, props);

    const environmentName = getEnv('ENVIRONMENT_NAME');
    const serviceName = getEnv('PROJECT_NAME');
    const projectName = getEnv('PROJECT_NAME');

    this.lambdaConstruct = new CRLambdaAlbConstruct(this, getResourceName(projectName), {
      environmentName: environmentName,
      serviceName: serviceName,
      runtime: Runtime.DOTNET_6,
      codeBucket: getEnv('SOURCE_BUCKET'),
      codePath: getEnv('CODE_PATH') || `${projectName}/${projectName}.zip`,
      handler: 'AccountsSyncData::AccountsSyncData.Function::FunctionHandler',
      visibility: ServiceVisibility.PRIVATE,
      memorySize: +getEnv('memoryLimitMiB'),
      timeout:+getEnv('timeOut'),
      provisionedConcurrentExecutions:+getEnv('ConcurrentExecutions'),
      autoScale: {
        maxCapacity: +getEnv('ScalingMaxCapacity'),
        minCapacity: +getEnv('ScalingMinCapacity'),
        utilizationTarget: +getEnv('ScalingUtilizationTarget')
      },
      functionDescription: getEnv('VERSION'),
    });

    this.lambdaConstruct.lambdaFunction.role?.addToPrincipalPolicy(new PolicyStatement({
      actions: [ "SNS:ListTopics" ],
      resources: [ "*" ]
   }));

    this.accountsTopicSub = new DomainTopicSubscriptionConstruct(this, getResourceName("MemberTopicSubscription"), {
      domain: "Member",
      environmentName: environmentName,
      serviceName:serviceName,
      maximumMessageCount: +getEnv('MaximumMessageCount'),
      // Filter for the events you're interested in, if blank, you'll get everything
      filterPolicy: DomainEventFilter.ByEventType(["MemberDetailChanged"]) 
    });

    this.accountsTopicSub.messageQueue.grantConsumeMessages(this.lambdaConstruct.lambdaFunction);
    this.accountsTopicSub.messageQueue.grantPurge(this.lambdaConstruct.lambdaFunction);

    this.lambdaConstruct.node.addDependency(this.accountsTopicSub);

    applyMetaTags(this, {'Team':'Accounts', 'Application': 'AccountsSyncData'});
  }
}
